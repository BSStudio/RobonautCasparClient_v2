using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using RobonautCasparClient_v2.DO;
using RobonautCasparClient_v2.DO.communication;
using RobonautCasparClient_v2.modules.interfaces;
using RobonautCasparClient_v2.Modules.interfaces;

namespace RobonautCasparClient_v2.modules.controller
{
    public class MainController
    {
        #region Singleton implementation

        private static MainController instance;

        public static MainController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainController();
                }

                return instance;
            }
        }

        #endregion

        private MainWindow _window;

        private IGraphicsServerInteractor graphicsInteractor = CasparServerInteractor.Instance;
        //private IDataServerInteractor dataInteractor = WebSocketDataInteractor.Instance;
        private IDataServerInteractor dataInteractor = RabbitMqDataInteractor.Instance;
        private TeamDataService teamDataService = TeamDataService.Instance;

        private readonly Stopwatch techTimerCounter = new Stopwatch();
        private readonly DispatcherTimer techTimerUpdateTimer;
        private int technicalStartTime = 20000;

        private readonly Stopwatch speedTimerCounter = new Stopwatch();
        private readonly DispatcherTimer speedTimerUpdateTimer;

        private bool FullscreenGraphicsShown { get; set; }
        private bool TimingShown { get; set; }
        private TimerType ShownTimingType { get; set; }
        private bool TechTimingRolling { get; set; }
        private int LastTechTimerTime { get; set; }
        private bool SpeedTimingRolling { get; set; }
        private int LastSpeedTimerTime { get; set; }

        public bool ConnectedToGraphicsServer
        {
            get { return graphicsInteractor.getConnectionToServer(); }
        }

        public MainWindow Window
        {
            get { return _window; }

            set
            {
                _window = value;

                graphicsInteractor.casparConnected += _window.graphicsServerConnected;
                graphicsInteractor.casparDisconnected += _window.graphicsServerDisconnected;
                dataInteractor.connectedEvent += _window.dataServerConnected;
                dataInteractor.connectionFailedEvent += _window.dataServerConnectionFailed;
                dataInteractor.disconnectedEvent += _window.dataServerDisconnected;

                dataInteractor.dataProviderClientConnectedEvent += _window.dataClientConnected;
                dataInteractor.dataProviderClientDisconnectedEvent += _window.dataClientDisconnected;
                dataInteractor.teamDatasRecievedEvent += _window.UpdateTeamData;
            }
        }

        private MainController()
        {
            TechTimingRolling = false;
            SpeedTimingRolling = false;

            LastSpeedTimerTime = 0;
            LastTechTimerTime = 20000;
            
            ShownTimingType = TimerType.TECHNICAL;

            FullscreenGraphicsShown = false;

            speedTimerUpdateTimer = new DispatcherTimer();
            speedTimerUpdateTimer.Tick += updateSpeedTimerOnUi;
            speedTimerUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);

            techTimerUpdateTimer = new DispatcherTimer();
            techTimerUpdateTimer.Tick += updateTechTimerTick;
            speedTimerUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);

            dataInteractor.technicalScoreRecievedEvent += technicalScoreRecieved;
            dataInteractor.teamResultRecievedEvent += updateTeamSpeedDisplay;
            dataInteractor.teamResultRecievedEvent += updateUiDataFromResult;

            dataInteractor.techTimerStartEvent += startTechTimer;
            dataInteractor.techTimerStopEvent += stopTechTimer;
            dataInteractor.stopperStartEvent += startSpeedTimer;
            dataInteractor.stopperStopEvent += stopSpeedTimer;
        }

        public void connectToGraphicsServer(string url)
        {
            graphicsInteractor.connect(url);
        }

        public void disconnectFromGraphicsServer()
        {
            graphicsInteractor.disconnect();
        }

        public void connectToDataServer(string url)
        {
            dataInteractor.connect(url);
        }

        public void disconnectFromDataServer()
        {
            dataInteractor.disconnect();
        }

        public void sendYearToDataInteractor(int year)
        {
            dataInteractor.setYear(year);
        }

        private void updateUiDataFromResult(TeamResultDto teamResult)
        {
            teamDataService.updateWithTeamResult(teamResult);
            _window.UpdateTeamData(teamDataService.Teams);
        }

        public void showName(string name, string title)
        {
            graphicsInteractor.showNameInsert(name, title);
        }

        public void hideName()
        {
            graphicsInteractor.hideNameInsert();
        }

        public void hideTeamGraphics()
        {
            graphicsInteractor.stopTeamDataGraphics();
        }

        public void hideAllGraphics()
        {
            graphicsInteractor.stopAllGraphics();
            TimingShown = false;
        }

        public void showTeamNaming(int teamId)
        {
            TeamData team = teamDataService.getTeam(teamId);
            graphicsInteractor.showTeamNameWithMembers(team);
        }

        public void showTeamTechnicalContestDisplay(int teamId)
        {
            TeamData team = teamDataService.getTeam(teamId);
            graphicsInteractor.showTeamTechnicalContestDisplay(team);
        }

        private void technicalScoreRecieved(TechnicalScoreDto techScore)
        {
            //pontmegjelenítő frissítés
            TeamData team = teamDataService.updateWithTechScore(techScore);
            graphicsInteractor.updateTeamTechnicalContestDisplay(team);

            //óra UI frissítés
            technicalStartTime = techScore.TimeLeft;
            techTimerCounter.Restart();

            //óra grafikai frissítés
            if (TimingShown)
            {
                graphicsInteractor.showTimer(techScore.TimeLeft, TimerDirection.DOWN);
            }
        }

        public void showTeamSpeedContestDisplay(int teamId)
        {
            TeamData team = teamDataService.getTeam(teamId);
            graphicsInteractor.showTeamSpeedContestDisplay(team);
        }

        private void updateTeamSpeedDisplay(TeamResultDto teamResult)
        {
            TeamData team = teamDataService.updateWithTeamResult(teamResult);
            graphicsInteractor.updateTeamSpeedContestDisplay(team);
        }

        public void requestData()
        {
            dataInteractor.requestTeamDataUpdate();
        }

        public void showTeamAllStatsInsert(int teamId, TeamType rankType)
        {
            TeamData team = teamDataService.getTeam(teamId);
            graphicsInteractor.showTeamAllStats(team, rankType);
        }

        public void showTechTimer()
        {
            if (ConnectedToGraphicsServer)
            {
                var time = technicalStartTime - techTimerCounter.ElapsedMilliseconds;
                if (TechTimingRolling)
                    graphicsInteractor.showTimer((int) time, TimerDirection.DOWN);
                else
                    graphicsInteractor.showTimer(LastTechTimerTime, TimerDirection.STOP);

                TimingShown = true;
                ShownTimingType = TimerType.TECHNICAL;
            }
        }

        private void startTechTimer(int startTime)
        {
            technicalStartTime = startTime;
            techTimerCounter.Restart();
            techTimerUpdateTimer.Start();

            TechTimingRolling = true;

            if (TimingShown)
            {
                graphicsInteractor.showTimer(startTime, TimerDirection.DOWN);
            }
        }

        private void stopTechTimer(int stopTime)
        {
            techTimerCounter.Stop();
            techTimerUpdateTimer.Stop();

            TechTimingRolling = false;
            LastTechTimerTime = stopTime;

            if (TimingShown)
            {
                graphicsInteractor.showTimer(stopTime, TimerDirection.STOP);
            }

            _window.updateTechTimerDisplay(stopTime);
        }

        private void updateTechTimerTick(Object o, EventArgs e)
        {
            var time = technicalStartTime - techTimerCounter.ElapsedMilliseconds;
            if (time <= 0)
            {
                techTimerCounter.Stop();
                techTimerUpdateTimer.Stop();
                time = 0;
            }

            _window.updateTechTimerDisplay(time);
        }

        public void showSpeedTimer()
        {
            if (ConnectedToGraphicsServer)
            {
                var time = speedTimerCounter.ElapsedMilliseconds;
                if (SpeedTimingRolling)
                    graphicsInteractor.showTimer((int) time, TimerDirection.UP);
                else
                    graphicsInteractor.showTimer(LastSpeedTimerTime, TimerDirection.STOP);

                TimingShown = true;
                ShownTimingType = TimerType.SPEED;
            }
        }

        private void startSpeedTimer()
        {
            speedTimerCounter.Restart();
            speedTimerUpdateTimer.Start();

            SpeedTimingRolling = true;

            if (TimingShown && ShownTimingType == TimerType.SPEED)
            {
                graphicsInteractor.showTimer(0, TimerDirection.UP);
            }
        }

        private void stopSpeedTimer(int stopTime)
        {
            speedTimerCounter.Stop();
            speedTimerUpdateTimer.Stop();

            SpeedTimingRolling = false;
            LastSpeedTimerTime = stopTime;

            if (TimingShown && ShownTimingType == TimerType.SPEED)
            {
                graphicsInteractor.showTimer(stopTime, TimerDirection.STOP);
            }

            _window.updateSpeedTimerDisplay(stopTime);
        }

        public void hideTiming()
        {
            if (ConnectedToGraphicsServer)
            {
                graphicsInteractor.hideTimer();

                TimingShown = false;
            }
        }

        private void updateSpeedTimerOnUi(Object o, EventArgs e)
        {
            var time = speedTimerCounter.ElapsedMilliseconds;
            _window.updateSpeedTimerDisplay(time);
        }

        public void showFullScreenGraphics(FullScreenTableType type)
        {
            if (ConnectedToGraphicsServer)
            {
                graphicsInteractor.showFullscreenGraphics(type, teamDataService.Teams);
                FullscreenGraphicsShown = true;
            }
        }

        public void nextFullScreenPage()
        {
            if (FullscreenGraphicsShown)
            {
                FullscreenGraphicsShown = graphicsInteractor.stepFullScreenGraphics(teamDataService.Teams);
            }
        }

        public void hideFullScreenGraphics()
        {
            if (FullscreenGraphicsShown && ConnectedToGraphicsServer)
            {
                graphicsInteractor.hideFullscreenGraphics();
                FullscreenGraphicsShown = false;
            }
        }
    }
}