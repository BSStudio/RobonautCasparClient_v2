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

        private int shownTableItemsAmount_;
        public int ShownTableItemsAmount
        {
            get
            {
                return shownTableItemsAmount_;
            }
            set
            {
                shownTableItemsAmount_ = value;
                graphicsInteractor.setShownFullscreenGraphicsItemAmount(value);
                /*
                if (FullscreenGraphicsShown)
                {
                    graphicsInteractor.updateFullScreenGraphicsTable(teamDataService.getLastGivenTeams(shownTableItemsAmount_));
                }*/
            }
        }

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
                
                dataInteractor.teamDataRecievedEvent += updateTeamsOnUI;
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

            ShownTableItemsAmount = -1;

            speedTimerUpdateTimer = new DispatcherTimer();
            speedTimerUpdateTimer.Tick += updateSpeedTimerOnUi;
            speedTimerUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);

            techTimerUpdateTimer = new DispatcherTimer();
            techTimerUpdateTimer.Tick += updateTechTimerTick;
            speedTimerUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);

            dataInteractor.gateInfoRecievedEvent += GateInfoRecieved;
            dataInteractor.speedRaceScoreRecievedEvent += updateTeamSpeedDisplay;
            dataInteractor.safetyCarFollowRecievedEvent += updateSafetyCarInfoDisplayWithFollow;
            dataInteractor.safetyCarOvertakeRecievedEvent += updateSafetyCarInfoDisplayWithOvertake;

            dataInteractor.techTimerStartEvent += startTechTimer;
            dataInteractor.techTimerStopEvent += stopTechTimer;
            dataInteractor.stopperStartEvent += startSpeedTimer;
            dataInteractor.stopperStopEvent += stopSpeedTimer;
        }

        public void setGraphicsChannel(int channel)
        {
            graphicsInteractor.setGraphicsChannel(channel);
        }
        
        private void updateTeamsOnUI(TeamData newteamdata)
        {
            Window.UpdateTeamData(teamDataService.Teams);
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

        private void GateInfoRecieved(GateInformation gateInfo)
        {
            //pontmegjelenítő frissítés
            TeamData team = teamDataService.updateWithGateInfo(gateInfo);
            graphicsInteractor.updateTeamTechnicalContestDisplay(team);

            //óra UI frissítés
            technicalStartTime = gateInfo.TimeLeft;
            techTimerCounter.Restart();

            //óra grafikai frissítés
            if (TimingShown)
            {
                graphicsInteractor.showTimer(gateInfo.TimeLeft, TimerDirection.DOWN);
            }
        }

        public void showTeamSpeedContestDisplay(int teamId)
        {
            TeamData team = teamDataService.getTeam(teamId);
            graphicsInteractor.showTeamSpeedContestDisplay(team);
        }

        private void updateTeamSpeedDisplay(SpeedRaceScore speedRaceScore)
        {
            TeamData team = teamDataService.updateWithSpeedRaceScore(speedRaceScore);
            graphicsInteractor.updateTeamSpeedContestDisplay(team);
        }

        public void showSafetyCarInfoDisplay(int teamId)
        {
            TeamData team = teamDataService.getTeam(teamId);
            graphicsInteractor.showSafetyCarInfoDisplay(team);
        }

        public void hideSafetyCarInfoDisplay()
        {
            graphicsInteractor.hideSafetyCarInfoDisplay();
        }

        public void updateSafetyCarInfoDisplayWithFollow(SafetyCarFollowInformation followInformation)
        {
            TeamData team = teamDataService.updateWithSafetyCarFollowInfo(followInformation);
            graphicsInteractor.updateSafetyCarInfoDisplay(team);
        }

        public void updateSafetyCarInfoDisplayWithOvertake(SafetyCarOvertakeInformation overtakeInformation)
        {
            TeamData team = teamDataService.updateWithSafetyCarOvertakeInfo(overtakeInformation);
            graphicsInteractor.updateSafetyCarInfoDisplay(team);
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

        private void startSpeedTimer(int startMs)
        {
            speedTimerCounter.Restart();
            speedTimerUpdateTimer.Start();

            SpeedTimingRolling = true;

            if (TimingShown && ShownTimingType == TimerType.SPEED)
            {
                graphicsInteractor.showTimer(startMs, TimerDirection.UP);
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
                if (type == FullScreenTableType.FINAL || type == FullScreenTableType.FINAL_JUNIOR)
                {
                    graphicsInteractor.showFullscreenGraphics(type, teamDataService.getLastGivenTeams(ShownTableItemsAmount));
                }
                else
                {
                    graphicsInteractor.showFullscreenGraphics(type, teamDataService.Teams);
                }
                
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