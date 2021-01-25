using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Documents;
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

        public const int SCOREBOARD_ITEMS_PER_PAGE = 6;
        public const int CHANNEL = 0; //1
        public const int NAME_LAYER = 10;
        public const int TEAMINFO_LAYER = 11;
        public const int SAFETY_CAR_INFO_LAYER = 12;
        public const int TIMER_LAYER = 13;
        public const int SCOREBOARD_LAYER = 14;

        private bool TimerShown { get; set; }
        private bool TechDisplayShown { get; set; }
        private bool SpeedDisplayShown { get; set; }
        private bool SafetyCarDisplayShown { get; set; }
        private bool FullScreenGraphicsShown { get; set; }
        private FullScreenTableType FullScreenGraphicsTypeShown { get; set; }
        private int CurrentFullScreenPage { get; set; }
        private int ShownItemsAmount { get; set; }

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
            SafetyCarDisplayShown = false;
            FullScreenGraphicsShown = false;
            CurrentFullScreenPage = 0;

            casparDevice.ConnectionStatusChanged += CasparDevice_ConnectionStatusChanged;
        }

        public override void connect(string connectionUrl)
        {
            if (!casparDevice.IsConnected)
            {
                casparDevice.Settings.Hostname = connectionUrl;
                casparDevice.Settings.Port = 5250;
                casparDevice.Connect();
            }
        }

        public override void disconnect()
        {
            casparDevice.Disconnect();
        }

        private void CasparDevice_ConnectionStatusChanged(object sender, ConnectionEventArgs e)
        {
            if (IsConnected)
            {
                fireCasparConnected();
            }
            else
            {
                fireCasparDisconnected();
            }
        }

        public override bool getConnectionToServer()
        {
            return IsConnected;
        }

        public override void setShownFullscreenGraphicsItemAmount(int amount)
        {
            ShownItemsAmount = amount;
        }

        public override void showNameInsert(string name, string title)
        {
            if (IsConnected)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("person_name", name);
                cgData.SetData("title", title);

                casparDevice.Channels[CHANNEL].CG.Add(NAME_LAYER, 0, "NEVINZERT", true, cgData);
            }
        }

        public override void hideNameInsert()
        {
            stopLayer(NAME_LAYER);
        }

        public override void showTeamNameWithMembers(TeamData teamData)
        {
            if (IsConnected)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("teamname", teamData.TeamName);
                cgData.SetData("members", teamData.TeamMembersString);

                casparDevice.Channels[CHANNEL].CG.Add(TEAMINFO_LAYER, 0, "CSAPAT_NEVINZERT", true, cgData);
            }
        }

        public override void stopTeamDataGraphics()
        {
            stopLayer(TEAMINFO_LAYER);
            stopLayer(SAFETY_CAR_INFO_LAYER);
        }

        public override void stopAllGraphics()
        {
            stopLayer(NAME_LAYER);
            stopLayer(TEAMINFO_LAYER);
            stopLayer(TIMER_LAYER);
            stopLayer(SCOREBOARD_LAYER);
            stopLayer(SAFETY_CAR_INFO_LAYER);
        }

        public override void showTeamTechnicalContestDisplay(TeamData teamData)
        {
            if (IsConnected)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("teamname", teamData.TeamName);
                cgData.SetData("point", teamData.SkillScore.ToString());

                casparDevice.Channels[CHANNEL].CG.Add(TEAMINFO_LAYER, 0, "CSAPAT_UGYESSEGI", true, cgData);

                TechDisplayShown = true;
            }
        }

        public override void updateTeamTechnicalContestDisplay(TeamData teamData)
        {
            if (IsConnected && TechDisplayShown)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("point", teamData.SkillScore.ToString());

                casparDevice.Channels[CHANNEL].CG.Update(TEAMINFO_LAYER, 0, cgData);
            }
        }

        public override void showTeamSpeedContestDisplay(TeamData teamData)
        {
            if (IsConnected)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("tech_point", teamData.SkillScore.ToString());

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

        public override void updateTeamSpeedContestDisplay(TeamData teamData)
        {
            if (IsConnected && SpeedDisplayShown)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("tech_point", teamData.SkillScore.ToString());

                int lap = 1;
                foreach (var time in teamData.SpeedTimes)
                {
                    cgData.SetData("time_" + lap, Converters.TimeToString(time));
                    lap++;
                }

                casparDevice.Channels[CHANNEL].CG.Update(TEAMINFO_LAYER, 0, cgData);
            }
        }

        public override void showSafetyCarInfoDisplay(TeamData teamData)
        {
            if (IsConnected && !SafetyCarDisplayShown)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("overtake", teamData.NumberOfOvertakes.ToString());
                cgData.SetData("follow", teamData.SafetyCarWasFollowed.ToString());
                
                casparDevice.Channels[CHANNEL].CG.Add(SAFETY_CAR_INFO_LAYER, 0, "ROBONAUT_SAFETY_CAR_INFO", true, cgData);
                
                SafetyCarDisplayShown = true;
            }
        }

        public override void updateSafetyCarInfoDisplay(TeamData teamData)
        {
            if (IsConnected && SafetyCarDisplayShown)
            {
                CasparCGDataCollection cgData = new CasparCGDataCollection();
                cgData.SetData("overtake", teamData.NumberOfOvertakes.ToString());
                cgData.SetData("follow", teamData.SafetyCarWasFollowed.ToString());
                
                casparDevice.Channels[CHANNEL].CG.Update(SAFETY_CAR_INFO_LAYER, 0, cgData);
            }
        }

        public override void hideSafetyCarInfoDisplay()
        {
            if (IsConnected && SafetyCarDisplayShown)
            {
                stopLayer(SAFETY_CAR_INFO_LAYER);
            }
        }

        public override void showTimer(int startMs, TimerDirection dir)
        {
            if (IsConnected)
            {
                if (!TimerShown)
                {
                    CasparCGDataCollection cgData = new CasparCGDataCollection();
                    cgData.SetData("dir", Converters.DirectionToString(dir));
                    cgData.SetData("time", startMs.ToString());

                    casparDevice.Channels[CHANNEL].CG.Add(TIMER_LAYER, 0, "ROBONAUT_TIMER", true, cgData);

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

        public override void hideTimer()
        {
            stopLayer(TIMER_LAYER);
        }

        public override void showTeamAllStats(TeamData teamData, TeamType rankType)
        {
            if (IsConnected)
            {
                int rank = 0;

                switch (rankType)
                {
                    case TeamType.JUNIOR:
                        rank = teamData.JuniorRank;
                        break;
                    case TeamType.SENIOR:
                        rank = teamData.Rank;
                        break;
                }

                CasparCGDataCollection CGdata = new CasparCGDataCollection();
                CGdata.SetData("teamname", teamData.TeamName);
                CGdata.SetData("teammembers", teamData.TeamMembersString);
                CGdata.SetData("broughtpoint", "Hozott pont: " + teamData.QualificationScore);
                CGdata.SetData("sumskillpoint", "Ügyességi pont: " + teamData.SkillScore);
                CGdata.SetData("sumspeedpoint", "Gyorsasági pont: " + teamData.SpeedScore);
                CGdata.SetData("audiencepoint", "Közönség pont: " + teamData.AudienceScore);
                CGdata.SetData("sumpoint", "Összpontszám: " + teamData.TotalScore);
                CGdata.SetData("rank", rank.ToString());

                casparDevice.Channels[CHANNEL].CG.Add(TEAMINFO_LAYER, 0, "CSAPAT_OSSZETETT", true, CGdata);
            }
        }

        public override void showFullscreenGraphics(FullScreenTableType type, List<TeamData> teamDatas)
        {
            if (IsConnected)
            {
                CurrentFullScreenPage = 0;
                CasparCGDataCollection cgData = new CasparCGDataCollection();

                FullScreenGraphicsTypeShown = type;

                switch (type)
                {
                    case FullScreenTableType.QUALIFICATION_POINTS:
                        cgData.SetData("title", "Hozott pontok");
                        updateQualificationCgData(cgData, teamDatas);

                        casparDevice.Channels[CHANNEL].CG.Add(SCOREBOARD_LAYER, 0, "RANGSOR_PONT", true, cgData);
                        break;
                    case FullScreenTableType.AUDIENCE_POINTS:
                        updateAudienceCgData(cgData, teamDatas);

                        casparDevice.Channels[CHANNEL].CG.Add(SCOREBOARD_LAYER, 0, "RANGSOR_KOZONSEG", true, cgData);
                        break;
                    case FullScreenTableType.TECHNICAL_POINTS:
                        cgData.SetData("title", "Ügyességi verseny rangsor");
                        updateTechnicalPointsCgData(cgData, teamDatas);
                        
                        casparDevice.Channels[CHANNEL].CG.Add(SCOREBOARD_LAYER, 0, "RANGSOR_PONT", true, cgData);
                        break;
                    case FullScreenTableType.SPEED_TIMES:
                        updateSpeedTimesCgData(cgData, teamDatas);
                        
                        casparDevice.Channels[CHANNEL].CG.Add(SCOREBOARD_LAYER, 0, "RANGSOR_GYORSASAGI", true, cgData);
                        break;
                    case FullScreenTableType.SPEED_POINTS:
                        cgData.SetData("title", "Gyorsasági verseny rangsor");
                        updateSpeedPointsCgData(cgData, teamDatas);
                        
                        casparDevice.Channels[CHANNEL].CG.Add(SCOREBOARD_LAYER, 0, "RANGSOR_PONT", true, cgData);
                        break;
                    case FullScreenTableType.FINAL_JUNIOR:
                        cgData.SetData("title", "Összesített junior rangsor");
                        updateJuniorFinalResultCgData(cgData, teamDatas);

                        casparDevice.Channels[CHANNEL].CG.Add(SCOREBOARD_LAYER, 0, "RANGSOR_PONT", true, cgData);
                        break;
                    case FullScreenTableType.FINAL:
                        cgData.SetData("title", "Összesített rangsor");
                        updateFinalResultCgData(cgData, teamDatas);

                        casparDevice.Channels[CHANNEL].CG.Add(SCOREBOARD_LAYER, 0, "RANGSOR_PONT", true, cgData);
                        break;
                }

                FullScreenGraphicsShown = true;
            }
        }

        public override bool stepFullScreenGraphics(List<TeamData> teamDatas)
        {
            if (IsConnected)
            {
                if (FullScreenGraphicsShown)
                {
                    var numOfTeams = FullScreenGraphicsTypeShown == FullScreenTableType.FINAL_JUNIOR
                        ? teamDatas.Where(team => team.TeamType == TeamType.JUNIOR).ToList().Count
                        : teamDatas.Count;
                    
                    var numberOfPages = (int) Math.Ceiling((double) numOfTeams / SCOREBOARD_ITEMS_PER_PAGE);

                    CurrentFullScreenPage++;

                    //0-tól indul az oldalak szamolasa
                    if (CurrentFullScreenPage >= numberOfPages)
                    {
                        stopLayer(SCOREBOARD_LAYER);
                    }
                    else
                    {
                        CasparCGDataCollection cgData = new CasparCGDataCollection();

                        switch (FullScreenGraphicsTypeShown)
                        {
                            case FullScreenTableType.QUALIFICATION_POINTS:
                                updateQualificationCgData(cgData, teamDatas);
                                break;
                            case FullScreenTableType.AUDIENCE_POINTS:
                                updateAudienceCgData(cgData, teamDatas);
                                break;
                            case FullScreenTableType.TECHNICAL_POINTS:
                                updateTechnicalPointsCgData(cgData, teamDatas);
                                break;
                            case FullScreenTableType.SPEED_TIMES:
                                updateSpeedTimesCgData(cgData, teamDatas);
                                break;
                            case FullScreenTableType.SPEED_POINTS:
                                updateSpeedPointsCgData(cgData, teamDatas);
                                break;
                            case FullScreenTableType.FINAL_JUNIOR:
                                updateJuniorFinalResultCgData(cgData, teamDatas);
                                break;
                            case FullScreenTableType.FINAL:
                                updateFinalResultCgData(cgData, teamDatas);
                                break;
                        }

                        casparDevice.Channels[CHANNEL].CG.Update(SCOREBOARD_LAYER, 0, cgData);
                    }
                }
            }

            return FullScreenGraphicsShown;
        }

        private void updateQualificationCgData(CasparCGDataCollection cgData, List<TeamData> teamDatas)
        {
            teamDatas.Sort((a, b) => b.QualificationScore - a.QualificationScore);

            int firstElement = CurrentFullScreenPage * SCOREBOARD_ITEMS_PER_PAGE;
            int lastElement = firstElement + SCOREBOARD_ITEMS_PER_PAGE <= teamDatas.Count
                ? firstElement + SCOREBOARD_ITEMS_PER_PAGE
                : teamDatas.Count;

            int cgIndex = 1;
            for (int i = firstElement; i < firstElement + SCOREBOARD_ITEMS_PER_PAGE; i++)
            {
                if (i < lastElement)
                {
                    var currentTeam = teamDatas[i];

                    cgData.SetData("result_rank_" + cgIndex, (i + 1).ToString());
                    cgData.SetData("result_teamname_" + cgIndex, currentTeam.TeamName);
                    cgData.SetData("result_point_" + cgIndex, currentTeam.QualificationScore.ToString());
                }
                else
                {
                    cgData.SetData("result_rank_" + cgIndex, "");
                    cgData.SetData("result_teamname_" + cgIndex, "");
                    cgData.SetData("result_point_" + cgIndex, "");
                }

                cgIndex++;
            }
        }

        private void updateTechnicalPointsCgData(CasparCGDataCollection cgData, List<TeamData> teamDatas)
        {
            teamDatas.Sort((a, b) => b.SkillScore - a.SkillScore);

            int firstElement = CurrentFullScreenPage * SCOREBOARD_ITEMS_PER_PAGE;
            int lastElement = firstElement + SCOREBOARD_ITEMS_PER_PAGE <= teamDatas.Count
                ? firstElement + SCOREBOARD_ITEMS_PER_PAGE
                : teamDatas.Count;

            int cgIndex = 1;
            for (int i = firstElement; i < firstElement + SCOREBOARD_ITEMS_PER_PAGE; i++)
            {
                if (i < lastElement)
                {
                    var currentTeam = teamDatas[i];

                    cgData.SetData("result_rank_" + cgIndex, (i + 1).ToString());
                    cgData.SetData("result_teamname_" + cgIndex, currentTeam.TeamName);
                    cgData.SetData("result_point_" + cgIndex, currentTeam.SkillScore.ToString());
                }
                else
                {
                    cgData.SetData("result_rank_" + cgIndex, "");
                    cgData.SetData("result_teamname_" + cgIndex, "");
                    cgData.SetData("result_point_" + cgIndex, "");
                }

                cgIndex++;
            }
        }

        private void updateSpeedTimesCgData(CasparCGDataCollection cgData, List<TeamData> teamDatas)
        {
            teamDatas.Sort((a, b) => a.FastestTime - b.FastestTime);

            int firstElement = CurrentFullScreenPage * SCOREBOARD_ITEMS_PER_PAGE;
            int lastElement = firstElement + SCOREBOARD_ITEMS_PER_PAGE <= teamDatas.Count
                ? firstElement + SCOREBOARD_ITEMS_PER_PAGE
                : teamDatas.Count;

            int cgIndex = 1;
            for (int i = firstElement; i < firstElement + SCOREBOARD_ITEMS_PER_PAGE; i++)
            {
                if (i < lastElement)
                {
                    var currentTeam = teamDatas[i];

                    cgData.SetData("result_rank_" + cgIndex, (i + 1).ToString());
                    cgData.SetData("result_teamname_" + cgIndex, currentTeam.TeamName);
                    cgData.SetData("result_time_" + cgIndex, Converters.TimeToString(currentTeam.FastestTime));
                }
                else
                {
                    cgData.SetData("result_rank_" + cgIndex, "");
                    cgData.SetData("result_teamname_" + cgIndex, "");
                    cgData.SetData("result_time_" + cgIndex, "");
                }

                cgIndex++;
            }
        }

        private void updateSpeedPointsCgData(CasparCGDataCollection cgData, List<TeamData> teamDatas)
        {
            teamDatas.Sort((a, b) => b.SumSpeedPoints - a.SumSpeedPoints);

            int firstElement = CurrentFullScreenPage * SCOREBOARD_ITEMS_PER_PAGE;
            int lastElement = firstElement + SCOREBOARD_ITEMS_PER_PAGE <= teamDatas.Count
                ? firstElement + SCOREBOARD_ITEMS_PER_PAGE
                : teamDatas.Count;

            int cgIndex = 1;
            for (int i = firstElement; i < firstElement + SCOREBOARD_ITEMS_PER_PAGE; i++)
            {
                if (i < lastElement)
                {
                    var currentTeam = teamDatas[i];

                    cgData.SetData("result_rank_" + cgIndex, (i + 1).ToString());
                    cgData.SetData("result_teamname_" + cgIndex, currentTeam.TeamName);
                    cgData.SetData("result_point_" + cgIndex, currentTeam.SumSpeedPoints.ToString());
                }
                else
                {
                    cgData.SetData("result_rank_" + cgIndex, "");
                    cgData.SetData("result_teamname_" + cgIndex, "");
                    cgData.SetData("result_point_" + cgIndex, "");
                }

                cgIndex++;
            }
        }

        private void updateAudienceCgData(CasparCGDataCollection cgData, List<TeamData> teamDatas)
        {
            teamDatas.Sort((a, b) => b.Votes - a.Votes);

            int firstElement = CurrentFullScreenPage * SCOREBOARD_ITEMS_PER_PAGE;
            int lastElement = firstElement + SCOREBOARD_ITEMS_PER_PAGE <= teamDatas.Count
                ? firstElement + SCOREBOARD_ITEMS_PER_PAGE
                : teamDatas.Count;

            int cgIndex = 1;
            for (int i = firstElement; i < firstElement + SCOREBOARD_ITEMS_PER_PAGE; i++)
            {
                if (i < lastElement)
                {
                    var currentTeam = teamDatas[i];

                    cgData.SetData("result_rank_" + cgIndex, (i + 1).ToString());
                    cgData.SetData("result_teamname_" + cgIndex, currentTeam.TeamName);
                    cgData.SetData("result_votes_" + cgIndex, currentTeam.Votes.ToString());
                    cgData.SetData("result_point_" + cgIndex, currentTeam.AudienceScore.ToString());
                }
                else
                {
                    cgData.SetData("result_rank_" + cgIndex, "");
                    cgData.SetData("result_teamname_" + cgIndex, "");
                    cgData.SetData("result_votes_" + cgIndex, "");
                    cgData.SetData("result_point_" + cgIndex, "");
                }

                cgIndex++;
            }
        }

        private void updateJuniorFinalResultCgData(CasparCGDataCollection cgData, List<TeamData> teamDatas)
        {
            teamDatas = teamDatas.Where(team => team.TeamType == TeamType.JUNIOR).ToList();
            teamDatas.Sort((a, b) => a.JuniorRank - b.JuniorRank);

            int firstElement = CurrentFullScreenPage * SCOREBOARD_ITEMS_PER_PAGE;
            int lastElement = firstElement + SCOREBOARD_ITEMS_PER_PAGE <= teamDatas.Count
                ? firstElement + SCOREBOARD_ITEMS_PER_PAGE
                : teamDatas.Count;

            int cgIndex = 1;
            for (int i = firstElement; i < firstElement + SCOREBOARD_ITEMS_PER_PAGE; i++)
            {
                if (i < lastElement)
                {
                    var currentTeam = teamDatas[i];

                    cgData.SetData("result_rank_" + cgIndex, currentTeam.JuniorRank.ToString());
                    cgData.SetData("result_teamname_" + cgIndex, currentTeam.TeamName);
                    cgData.SetData("result_point_" + cgIndex, currentTeam.TotalScore.ToString());
                }
                else
                {
                    cgData.SetData("result_rank_" + cgIndex, "");
                    cgData.SetData("result_teamname_" + cgIndex, "");
                    cgData.SetData("result_point_" + cgIndex, "");
                }

                cgIndex++;
            }
        }

        private void updateFinalResultCgData(CasparCGDataCollection cgData, List<TeamData> teamDatas)
        {
            teamDatas.Sort((a, b) => a.Rank - b.Rank);

            int firstElement = CurrentFullScreenPage * SCOREBOARD_ITEMS_PER_PAGE;
            int lastElement = firstElement + SCOREBOARD_ITEMS_PER_PAGE <= teamDatas.Count
                ? firstElement + SCOREBOARD_ITEMS_PER_PAGE
                : teamDatas.Count;

            int cgIndex = 1;
            for (int i = firstElement; i < firstElement + SCOREBOARD_ITEMS_PER_PAGE; i++)
            {
                if (i < lastElement)
                {
                    var currentTeam = teamDatas[i];

                    cgData.SetData("result_rank_" + cgIndex, currentTeam.Rank.ToString());
                    cgData.SetData("result_teamname_" + cgIndex, currentTeam.TeamName);
                    cgData.SetData("result_point_" + cgIndex, currentTeam.TotalScore.ToString());
                }
                else
                {
                    cgData.SetData("result_rank_" + cgIndex, "");
                    cgData.SetData("result_teamname_" + cgIndex, "");
                    cgData.SetData("result_point_" + cgIndex, "");
                }

                cgIndex++;
            }
        }

        public override void hideFullscreenGraphics()
        {
            stopLayer(SCOREBOARD_LAYER);
        }

        private void stopLayer(int layer)
        {
            if (IsConnected)
            {
                casparDevice.Channels[CHANNEL].CG.Stop(layer, 0);

                switch (layer)
                {
                    case TIMER_LAYER:
                        TimerShown = false;
                        break;
                    case TEAMINFO_LAYER:
                        SpeedDisplayShown = false;
                        TechDisplayShown = false;
                        break;
                    case SCOREBOARD_LAYER:
                        FullScreenGraphicsShown = false;
                        break;
                    case SAFETY_CAR_INFO_LAYER:
                        SafetyCarDisplayShown = false;
                        break;
                }
            }
        }
    }
}