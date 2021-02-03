using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RobonautCasparClient_v2.DO;
using RobonautCasparClient_v2.modules.controller;

namespace RobonautCasparClient_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainController mainController = MainController.Instance;

        private bool ConnectedToGraphicsServer { get; set; } = false;
        private bool ConnectedToDataServer { get; set; } = false;
        private bool TeamGraphicsShown { get; set; } = false;
        private bool SafetyCarInfoShown { get; set; } = false;
        private int ShownTeamsAmount { get; set; } = 0;
        public FullScreenTableType LastShownTableType { get; set; }

        private Button selectedTeamGraphicsButton = null;
        private Button selectedSafetyCarGraphicsButton = null;
        private Button selectedResultTableButton = null;
        private readonly Brush activeButtonBrush = new SolidColorBrush(Colors.ForestGreen);

        public MainWindow()
        {
            InitializeComponent();

            mainController.Window = this;

            this.PreviewKeyUp += new KeyEventHandler(handleEsc);
        }

        private void handleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                hideAllGraphics();
            }
        }

        public void dataServerConnected()
        {
            Dispatcher.Invoke(() =>
            {
                dataServerStatus.Text = "Csatlakozva";
                dataServerConnectButton.Content = "Kapcsolat bontása";
                dataServerConnectionFeedback.Fill = new SolidColorBrush(Colors.Green);
            });
            ConnectedToDataServer = true;
        }

        public void dataServerDisconnected()
        {
            Dispatcher.Invoke(() =>
            {
                dataServerStatus.Text = "Kapcsolat bontva";
                dataServerConnectButton.Content = "Csatlakozás";
                ConnectedToDataServer = false;
                dataServerConnectionFeedback.Fill = new SolidColorBrush(Colors.Tomato);
            });
            ConnectedToDataServer = false;
        }

        public void dataServerConnectionFailed()
        {
            MessageBox.Show("A csatlakozás nem sikerült a szerverhez!", "Hiba");
        }

        public void graphicsServerConnected()
        {
            Dispatcher.Invoke(() =>
            {
                casparServerStatus.Text = "Csatlakozva";
                casparServerConnectButton.Content = "Kapcsolat bontása";
                casparServerConnectionFeedback.Fill = new SolidColorBrush(Colors.Green);
            });

            ConnectedToGraphicsServer = true;
        }

        public void graphicsServerDisconnected()
        {
            Dispatcher.Invoke(() =>
            {
                casparServerStatus.Text = "Kapcsolat bontva";
                casparServerConnectButton.Content = "Csatlakozás";
                mainController.disconnectFromGraphicsServer();
                casparServerConnectionFeedback.Fill = new SolidColorBrush(Colors.Tomato);
            });

            ConnectedToGraphicsServer = false;
        }

        public void UpdateTeamData(List<TeamWithRanks> teamDatas)
        {
            Dispatcher.Invoke(() => { teamsGrid.ItemsSource = teamDatas.OrderBy(c => c.TeamData.TeamId); });
        }

        private void graphicsServerConnectButton_Click(object sender, RoutedEventArgs e)
        {
            connectToGraphicsServer();
        }

        private void CasparServerAddress_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
                connectToGraphicsServer();
        }

        private void connectToGraphicsServer()
        {
            if (!ConnectedToGraphicsServer)
                if (casparServerAddress.Text != "")
                {
                    mainController.connectToGraphicsServer(casparServerAddress.Text);
                }
                else
                {
                    MessageBox.Show("Add meg a Caspar szerver URL-jét!", "Hiba");
                }
            else
                mainController.disconnectFromGraphicsServer();
        }

        private void dataServerConnectButton_Click(object sender, RoutedEventArgs e)
        {
            connectToDataServer();
        }
        
        private void DataServerAddress_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                connectToDataServer();
        }

        private void connectToDataServer()
        {
            if (!ConnectedToDataServer)
                if (dataServerAddress.Text != "")
                {
                    mainController.connectToDataServer(dataServerAddress.Text);
                }
                else
                {
                    MessageBox.Show("Add meg az adat szerver URL-jét!", "Hiba");
                }
            else
            {
                mainController.disconnectFromDataServer();
            }
        }

        private void addToNameListButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!namingName.Text.Equals(""))
            {
                NamesComboBox.Items.Add(namingName.Text + "-" + namingTitle.Text);

                namingName.Clear();
                namingTitle.Clear();
            }
        }

        private void showNameInsertButton_Click(object sender, RoutedEventArgs e)
        {
            if (namingName.Text.Equals(""))
            {
                if (!NamesComboBox.Text.Equals("-"))
                {
                    var name = NamesComboBox.Text.Split('-')[0];
                    var title = NamesComboBox.Text.Split('-')[1];

                    mainController.showName(name, title);
                }
            }
            else
            {
                mainController.showName(namingName.Text, namingTitle.Text);

                namingName.Clear();
                namingTitle.Clear();
            }
        }

        private void hideNameInsertButton_Click(object sender, RoutedEventArgs e) => mainController.hideName();

        private void showStopperButton_Click(object sender, RoutedEventArgs e) => mainController.showSpeedTimer();

        private void hideStopperButton_Click(object sender, RoutedEventArgs e) => mainController.hideTiming();

        private void showTechTimerButton_Click(object sender, RoutedEventArgs e) => mainController.showTechTimer();

        private void hideTechTimerButton_Click(object sender, RoutedEventArgs e) => mainController.hideTiming();

        private void removeTeamGraphicsButton_Click(object sender, RoutedEventArgs e)
        {
            hideTeamGraphicsWithSafety();

            resetColor(selectedTeamGraphicsButton);
            resetColor(selectedSafetyCarGraphicsButton);
        }

        private void removeAllGraphicsButton_Click(object sender, RoutedEventArgs e)
        {
            hideAllGraphics();
        }

        private void hideAllGraphics()
        {
            if (ConnectedToGraphicsServer)
            {
                mainController.hideAllGraphics();
                TeamGraphicsShown = false;
                SafetyCarInfoShown = false;

                resetColor(selectedTeamGraphicsButton);
                resetColor(selectedSafetyCarGraphicsButton);
            }
        }

        private void refreshDataButton_Click(object sender, RoutedEventArgs e) => mainController.requestData();

        private void teamNamingButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    var currentButton = (Button) sender;
                    setActiveTeamInfoButton(currentButton);

                    mainController.showTeamNaming((int) currentButton.Tag);
                    TeamGraphicsShown = true;
                }
                else
                {
                    hideTeamGraphics();
                }
        }

        private void showTechnicalContestResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    var currentButton = (Button) sender;
                    setActiveTeamInfoButton(currentButton);

                    mainController.showTeamTechnicalContestDisplay((int) currentButton.Tag);
                    TeamGraphicsShown = true;
                }
                else
                {
                    hideTeamGraphics();
                }
        }

        private void showSafetyCarInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!SafetyCarInfoShown)
                {
                    var currentButton = (Button) sender;
                    setActiveSafetyCarButton(currentButton);

                    mainController.showSafetyCarInfoDisplay((int) currentButton.Tag);
                    SafetyCarInfoShown = true;
                }
                else
                {
                    mainController.hideSafetyCarInfoDisplay();
                    SafetyCarInfoShown = false;

                    resetColor(selectedSafetyCarGraphicsButton);
                }
        }

        private void showSpeedContestResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    var currentButton = (Button) sender;
                    setActiveTeamInfoButton(currentButton);

                    mainController.showTeamSpeedContestDisplay((int) currentButton.Tag);
                    TeamGraphicsShown = true;
                }
                else
                {
                    hideTeamGraphics();
                }
        }

        private void showTeamResultJuniorRankButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    var currentButton = (Button) sender;
                    setActiveTeamInfoButton(currentButton);

                    mainController.showTeamAllStatsInsert((int) currentButton.Tag, TeamType.JUNIOR);
                    TeamGraphicsShown = true;
                }
                else
                {
                    hideTeamGraphics();
                }
        }

        private void showTeamResultTotalRankButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    var currentButton = (Button) sender;
                    setActiveTeamInfoButton(currentButton);

                    mainController.showTeamAllStatsInsert((int) currentButton.Tag, TeamType.SENIOR);
                    TeamGraphicsShown = true;
                }
                else
                {
                    hideTeamGraphics();
                }
        }

        private void showQualificationPointsTableButton_Click(object sender, RoutedEventArgs e)
        {
            showFullScreenGraphics(FullScreenTableType.QUALIFICATION_POINTS);
            setActiveResultTableButton((Button) sender);
        }

        private void showTotalAudiencePointsTableButton_Click(object sender, RoutedEventArgs e)
        {
            showFullScreenGraphics(FullScreenTableType.AUDIENCE_POINTS);
            setActiveResultTableButton((Button) sender);
        }

        private void showTechnicalPointsTableButton_Click(object sender, RoutedEventArgs e)
        {
            showFullScreenGraphics(FullScreenTableType.SKILL_POINTS);
            setActiveResultTableButton((Button) sender);
        }

        private void showSpeedTimesTableButton_Click(object sender, RoutedEventArgs e)
        {
            showFullScreenGraphics(FullScreenTableType.SPEED_TIMES);
            setActiveResultTableButton((Button) sender);
        }

        private void showSpeedPointsTableButton_Click(object sender, RoutedEventArgs e)
        {
            showFullScreenGraphics(FullScreenTableType.SPEED_POINTS);
            setActiveResultTableButton((Button) sender);
        }

        private void showJuniorFinalResultTableButton_Click(object sender, RoutedEventArgs e)
        {
            showFullScreenGraphics(FullScreenTableType.FINAL_JUNIOR);
            setActiveResultTableButton((Button) sender);
        }

        private void showTotalFinalResultTableButton_Click(object sender, RoutedEventArgs e)
        {
            showFullScreenGraphics(FullScreenTableType.FINAL);
            setActiveResultTableButton((Button) sender);
        }

        private void nextFullScreenGraphicsPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mainController.nextFullScreenPage(LastShownTableType))
                resetColor(selectedResultTableButton);
        }

        private void hideFullScreenGraphicsPageButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.hideFullScreenGraphics();
            resetColor(selectedResultTableButton);
        }

        private void showFullScreenGraphics(FullScreenTableType type)
        {
            mainController.showFullScreenGraphics(type);
            LastShownTableType = type;
        }

        public void updateTechTimerDisplay(long time)
        {
            Dispatcher.Invoke(() => { uiTechTimer.Text = Converters.TimeToString((int) time); });
        }

        public void updateSpeedTimerDisplay(long time)
        {
            Dispatcher.Invoke(() => { uiStopper.Text = Converters.TimeToString((int) time); });
        }

        private void hideTeamGraphicsWithSafety()
        {
            mainController.hideTeamGraphics();
            mainController.hideSafetyCarInfoDisplay();
            TeamGraphicsShown = false;
            SafetyCarInfoShown = false;
        }

        private void hideTeamGraphics()
        {
            mainController.hideTeamGraphics();
            TeamGraphicsShown = false;

            resetColor(selectedTeamGraphicsButton);
        }

        private void increaseShownTeamsInTable(object sender, RoutedEventArgs e)
        {
            var numOfTeams = TeamDataService.Instance.Teams.Count;
            ShownTeamsAmount++;
            if (ShownTeamsAmount > numOfTeams)
                ShownTeamsAmount = numOfTeams;

            mainController.ShownTableItemsAmount = ShownTeamsAmount;

            NumberOfShownTeamsInTable.Text = ShownTeamsAmount.ToString();
        }

        private void decreaseShownTeamsInTable(object sender, RoutedEventArgs e)
        {
            ShownTeamsAmount--;
            if (ShownTeamsAmount < 0)
                ShownTeamsAmount = 0;

            mainController.ShownTableItemsAmount = ShownTeamsAmount;

            if (ShownTeamsAmount == 0)
                NumberOfShownTeamsInTable.Text = "Összes";
            else
                NumberOfShownTeamsInTable.Text = ShownTeamsAmount.ToString();
        }

        private void setShowAllTeamsInTable(object sender, RoutedEventArgs e)
        {
            ShownTeamsAmount = 0;
            mainController.ShownTableItemsAmount = 0;

            NumberOfShownTeamsInTable.Text = "Összes";
        }

        private void CasparSetChannelButton_OnClick(object sender, RoutedEventArgs e)
        {
            SetCasparChannel();
        }

        private void CasparChannel_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
                SetCasparChannel();
        }

        private void SetCasparChannel()
        {
            var text = casparChannel.Text;

            if (int.TryParse(text, out int channel))
            {
                mainController.setGraphicsChannel(channel);
            }
            else
            {
                MessageBox.Show("Kérlek számot adj meg Caspar csatornaként!", "Hiba");
            }
        }

        private void setActiveSafetyCarButton(Button button)
        {
            if (ConnectedToGraphicsServer)
            {
                button.Background = activeButtonBrush;
                selectedSafetyCarGraphicsButton = button;
            }
        }

        private void setActiveTeamInfoButton(Button button)
        {
            if (ConnectedToGraphicsServer)
            {
                button.Background = activeButtonBrush;
                selectedTeamGraphicsButton = button;
            }
        }

        private void setActiveResultTableButton(Button button)
        {
            if (ConnectedToGraphicsServer)
            {
                resetColor(selectedResultTableButton);
                button.Background = activeButtonBrush;
                selectedResultTableButton = button;
            }
        }

        private void resetColor(Button button)
        {
            if (button != null)
                button.Background = new SolidColorBrush(Colors.LightGray);
        }
    }
}