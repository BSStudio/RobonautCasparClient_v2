using System.Collections.Generic;
using System.Timers;
using RobonautCasparClient_v2.DO;
using RobonautCasparClient_v2.Modules.interfaces;
using Svt.Caspar;
using Svt.Network;

namespace RobonautCasparClient_v2.modules
{
    public class CasparServerInteractor : IGraphicsServerInteractor
    {
        #region Singleton implementation

        private static CasparServerInteractor instance;

        public static CasparServerInteractor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CasparServerInteractor();
                }

                return instance;
            }
        }

        #endregion

        public static int SCOREBOARD_ITEMS_PER_PAGE = 6;
        public static int CHANNEL = 0; //1
        public static int NAME_LAYER = 10;
        public static int TEAMINFO_LAYER = 11;
        public static int TIMER_LAYER = 12;
        public static int SCOREBOARD_LAYER = 13;
        public static int BREAK_BETWEEN_PAGES = 7000;

        public delegate void casparConnectedDelegate();

        public event casparConnectedDelegate casparConnected;

        public delegate void casparDisconnectedDelegate();

        public event casparDisconnectedDelegate casparDisconnected;

        private bool TimerShown { get; set; }
        private bool TechDisplayShown { get; set; }
        private bool SpeedDisplayShown { get; set; }

        private readonly CasparDevice casparDevice = new CasparDevice();

        public bool IsConnected
        {
            get { return casparDevice.IsConnected; }
        }

        private CasparServerInteractor()
        {
            TimerShown = false;
            TechDisplayShown = false;
            SpeedDisplayShown = false;

            casparDevice.ConnectionStatusChanged += CasparDevice_ConnectionStatusChanged;
        }

        public void connect(string connectionUrl)
        {
            if (!casparDevice.IsConnected)
            {
                casparDevice.Settings.Hostname = connectionUrl;
                casparDevice.Settings.Port = 5250;
                casparDevice.Connect();
            }
        }

        public void disconnect()
        {
            casparDevice.Disconnect();
        }

        private void CasparDevice_ConnectionStatusChanged(object sender, ConnectionEventArgs e)
        {
            if (IsConnected)
            {
                casparConnected();
            }
            else
            {
                casparDisconnected();
            }
        }

        public void showNameInsert(string name, string title)
        {
            if (IsConnected)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("person_name", name);
                cgData.SetData("title", title);

                casparDevice.Channels[CHANNEL].CG.Add(NAME_LAYER, 0, "NEVINZERT", true, cgData);
            }
        }

        public void hideNameInsert()
        {
            stopLayer(NAME_LAYER);
        }

        public void showTeamNameWithMembers(TeamData teamData)
        {
            if (IsConnected)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("teamname", teamData.TeamName);
                cgData.SetData("members", teamData.TeamMembersString);

                casparDevice.Channels[CHANNEL].CG.Add(TEAMINFO_LAYER, 0, "CSAPAT_NEVINZERT", true, cgData);
            }
        }

        public void stopTeamDataGraphics()
        {
            stopLayer(TEAMINFO_LAYER);

            TechDisplayShown = false;
            SpeedDisplayShown = false;
        }

        public void stopAllGraphics()
        {
            stopLayer(NAME_LAYER);
            stopLayer(TEAMINFO_LAYER);
            stopLayer(TIMER_LAYER);
            stopLayer(SCOREBOARD_LAYER);

            TimerShown = false;
            TechDisplayShown = false;
            SpeedDisplayShown = false;
        }

        public void showTeamTechnicalContestDisplay(TeamData teamData)
        {
            if (IsConnected)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("teamname", teamData.TeamName);
                cgData.SetData("point", teamData.TechnicalScore.ToString());

                casparDevice.Channels[CHANNEL].CG.Add(TEAMINFO_LAYER, 0, "CSAPAT_UGYESSEGI", true, cgData);

                TechDisplayShown = true;
            }
        }

        public void updateTeamTechnicalContestDisplay(TeamData teamData)
        {
            if (IsConnected && TechDisplayShown)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("point", teamData.TechnicalScore.ToString());

                casparDevice.Channels[CHANNEL].CG.Update(TEAMINFO_LAYER, 0, cgData);
            }
        }

        public void showTeamSpeedContestDisplay(TeamData teamData)
        {
            if (IsConnected)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("tech_point", teamData.TechnicalScore.ToString());

                int lap = 1;
                foreach (var time in teamData.SpeedTimes)
                {
                    cgData.SetData("time_" + lap, Converters.TimeToString(time));
                    lap++;
                }

                casparDevice.Channels[CHANNEL].CG.Add(TEAMINFO_LAYER, 0, "CSAPAT_GYORSASAGI", true, cgData);

                SpeedDisplayShown = true;
            }
        }

        public void updateTeamSpeedContestDisplay(TeamData teamData)
        {
            if (IsConnected && SpeedDisplayShown)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("tech_point", teamData.TechnicalScore.ToString());

                int lap = 1;
                foreach (var time in teamData.SpeedTimes)
                {
                    cgData.SetData("time_" + lap, Converters.TimeToString(time));
                    lap++;
                }

                casparDevice.Channels[CHANNEL].CG.Update(TEAMINFO_LAYER, 0, cgData);
            }
        }

        public void showTimer(int startMs, TimerDirection dir)
        {
            if (IsConnected)
            {
                if (!TimerShown)
                {
                    CasparCGDataCollection cgData = new CasparCGDataCollection();
                    cgData.SetData("dir", Converters.DirectionToString(dir));
                    cgData.SetData("time", startMs.ToString());

                    casparDevice.Channels[CHANNEL].CG.Add(TIMER_LAYER, 0, "ROBONAUT_TIMER_2021", true, cgData);

                    TimerShown = true;
                }
                else
                {
                    CasparCGDataCollection cgData = new CasparCGDataCollection();
                    cgData.SetData("dir", Converters.DirectionToString(dir));
                    cgData.SetData("time", startMs.ToString());

                    casparDevice.Channels[CHANNEL].CG.Update(TIMER_LAYER, 0, cgData);
                }
            }
        }

        public void hideTimer()
        {
            stopLayer(TIMER_LAYER);
            
            TimerShown = false;
        }

        public void showFullscreenTable(FullScreenTableType type, List<TeamData> teamDatas)
        {
            throw new System.NotImplementedException();
        }

        public void hideFullscreenTable()
        {
            stopLayer(SCOREBOARD_LAYER);
        }

        public void showTeamAllStats(TeamData teamData, TeamType rankType)
        {
            if (IsConnected)
            {
                int rank = 0;

                switch (rankType)
                {
                    case TeamType.JUNIOR:
                        rank = teamData.RankJunior;
                        break;
                    case TeamType.SENIOR:
                        rank = teamData.Rank;
                        break;
                }

                CasparCGDataCollection CGdata = new CasparCGDataCollection();
                CGdata.SetData("teamname", teamData.TeamName);
                CGdata.SetData("teammembers", teamData.TeamMembersString);
                CGdata.SetData("broughtpoint", "Hozott pont: " + teamData.QualificationScore);
                CGdata.SetData("sumskillpoint", "Ügyességi pont: " + teamData.TechnicalScore);
                CGdata.SetData("sumspeedpoint", "Gyorsasági pont: " + teamData.SpeedScore);
                CGdata.SetData("audiencepoint", "Közönség pont: " + teamData.AudienceScore);
                CGdata.SetData("sumpoint", "Összpontszám: " + teamData.TotalScore);
                CGdata.SetData("rank", rank.ToString());

                casparDevice.Channels[CHANNEL].CG.Add(TEAMINFO_LAYER, 0, "CSAPAT_OSSZETETT", true, CGdata);
            }
        }

        private void stopLayer(int layer)
        {
            if (IsConnected)
            {
                casparDevice.Channels[CHANNEL].CG.Stop(layer, 0);
            }
        }
    }
}