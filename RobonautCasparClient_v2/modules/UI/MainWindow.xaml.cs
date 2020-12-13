using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        private bool ConnectedToGraphicsServer { get; set; }
        private bool ConnectedToDataServer { get; set; }
        private bool TeamGraphicsShown { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            mainController.Window = this;

            CurrentYear.Text = DateTime.Now.Year.ToString();

            ConnectedToDataServer = false;
            ConnectedToGraphicsServer = false;
            TeamGraphicsShown = false;
        }

        public void dataServerConnected()
        {
            Dispatcher.Invoke(() =>
            {
                CurrentYear.IsEnabled = false;
                dataServerStatus.Text = "Csatlakozva";
                dataServerConnectButton.Content = "Kapcsolat bontása";
                dataServerConnectionFeedback.Fill = new SolidColorBrush(Colors.Green);

                clientConnectionStatusText.Content = "Versenyvezérlő nincs csatlakozva az adatszerverhez";
                clientConnectionStatus.Fill = new SolidColorBrush(Colors.Orange);
            });
            ConnectedToDataServer = true;
        }

        public void dataServerDisconnected()
        {
            Dispatcher.Invoke(() =>
            {
                CurrentYear.IsEnabled = true;

                dataServerStatus.Text = "Kapcsolat bontva";
                dataServerConnectButton.Content = "Csatlakozás";
                ConnectedToDataServer = false;
                dataServerConnectionFeedback.Fill = new SolidColorBrush(Colors.Tomato);

                clientConnectionStatusText.Content = "A feliratozó kliens nincs csatlakozva az adatszerverhez";
                clientConnectionStatus.Fill = new SolidColorBrush(Colors.Tomato);
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

        public void dataClientConnected()
        {
            Dispatcher.Invoke(() =>
            {
                clientConnectionStatusText.Content = "Versenyvezérlő csatlakozva az adatszerverhez";
                clientConnectionStatus.Fill = new SolidColorBrush(Colors.Green);
            });
        }

        public void dataClientDisconnected()
        {
            Dispatcher.Invoke(() =>
            {
                clientConnectionStatusText.Content = "Versenyvezérlő nincs csatlakozva az adatszerverhez";
                clientConnectionStatus.Fill = new SolidColorBrush(Colors.Orange);
            });
        }

        public void UpdateTeamData(List<TeamData> teamDatas)
        {
            Dispatcher.Invoke(() => { teamsGrid.ItemsSource = teamDatas.OrderBy(c => c.TeamId); });
        }

        private void graphicsServerConnectButton_Click(object sender, RoutedEventArgs e)
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

        private void yearUpdated(object sender, RoutedEventArgs e)
        {
            mainController.sendYearToDataInteractor(Int32.Parse(CurrentYear.Text));
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

        private void hideNameInsertButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.hideName();
        }

        private void showStopperButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.showSpeedTimer();
        }

        private void hideStopperButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.hideTiming();
        }

        private void showTechTimerButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.showTechTimer();
        }

        private void hideTechTimerButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.hideTiming();
        }

        private void removeTeamGraphicsButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.hideTeamGraphics();
            TeamGraphicsShown = false;
        }

        private void removeAllGraphicsButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.hideAllGraphics();
            TeamGraphicsShown = false;
        }

        private void refreshDataButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.requestData();
        }

        private void teamNamingButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    mainController.showTeamNaming((int) ((Button) sender).Tag);
                    TeamGraphicsShown = true;
                }
                else
                {
                    mainController.hideTeamGraphics();
                    TeamGraphicsShown = false;
                }
        }

        private void showTechnicalContestResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    mainController.showTeamTechnicalContestDisplay((int) ((Button) sender).Tag);
                    TeamGraphicsShown = true;
                }
                else
                {
                    mainController.hideTeamGraphics();
                    TeamGraphicsShown = false;
                }
        }

        private void showSpeedContestResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    mainController.showTeamSpeedContestDisplay((int) ((Button) sender).Tag);
                    TeamGraphicsShown = true;
                }
                else
                {
                    mainController.hideTeamGraphics();
                    TeamGraphicsShown = false;
                }
        }

        private void showTeamResultJuniorRankButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    mainController.showTeamAllStatsInsert((int) ((Button) sender).Tag, TeamType.JUNIOR);
                    TeamGraphicsShown = true;
                }
                else
                {
                    mainController.hideTeamGraphics();
                    TeamGraphicsShown = false;
                }
        }

        private void showTeamResultTotalRankButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainController.ConnectedToGraphicsServer)
                if (!TeamGraphicsShown)
                {
                    mainController.showTeamAllStatsInsert((int) ((Button) sender).Tag, TeamType.SENIOR);
                    TeamGraphicsShown = true;
                }
                else
                {
                    mainController.hideTeamGraphics();
                    TeamGraphicsShown = false;
                }
        }

        private void showQualificationPointsTableButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.showFullScreenGraphics(FullScreenTableType.QUALIFICATION_POINTS);
        }

        private void showTotalAudiencePointsTableButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.showFullScreenGraphics(FullScreenTableType.AUDIENCE_POINTS);
        }
        
        private void showTechnicalPointsTableButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.showFullScreenGraphics(FullScreenTableType.TECHNICAL_POINTS);
        }
        
        private void showSpeedTimesTableButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.showFullScreenGraphics(FullScreenTableType.SPEED_TIMES);
        }

        private void showJuniorFinalResultTableButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.showFullScreenGraphics(FullScreenTableType.FINAL_JUNIOR);
        }

        private void showTotalFinalResultTableButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.showFullScreenGraphics(FullScreenTableType.FINAL);
        }

        private void nextFullScreenGraphicsPageButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.nextFullScreenPage();
        }

        private void hideFullScreenGraphicsPageButton_Click(object sender, RoutedEventArgs e)
        {
            mainController.hideFullScreenGraphics();
        }

        public void updateTechTimerDisplay(long time)
        {
            Dispatcher.Invoke(() => { uiTechTimer.Text = Converters.TimeToString((int) time); });
        }

        public void updateSpeedTimerDisplay(long time)
        {
            Dispatcher.Invoke(() => { uiStopper.Text = Converters.TimeToString((int) time); });
        }
    }
}