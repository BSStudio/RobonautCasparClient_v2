using System.Collections.Generic;
using RobonautCasparClient_v2.DO.communication;

namespace RobonautCasparClient_v2.DO
{
    public class TeamDataService
    {
        private static TeamDataService _instance;

        public static TeamDataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TeamDataService();
                }

                return _instance;
            }
        }

        public List<TeamData> Teams { get; set; }

        private TeamDataService()
        {
            Teams = new List<TeamData>();
        }

        public void updateTeam(TeamData team)
        {
            var teamToUpdate = Teams.Find(team_ => { return team.TeamId == team_.TeamId && team.Year == team_.Year; });

            if (teamToUpdate == null)
            {
                Teams.Add(team);
            }
            else
            {
                teamToUpdate.TeamName = team.TeamName;
                teamToUpdate.TeamMembers = team.TeamMembers;
                teamToUpdate.TeamType = team.TeamType;
                teamToUpdate.Votes = team.Votes;
                teamToUpdate.AudienceScore = team.AudienceScore;
                teamToUpdate.QualificationScore = team.QualificationScore;
                teamToUpdate.NumberOfOvertakes = team.NumberOfOvertakes;
                teamToUpdate.SafetyCarWasFollowed = team.SafetyCarWasFollowed;
                teamToUpdate.SpeedScore = team.SpeedScore;
                teamToUpdate.SpeedTimes = team.SpeedTimes;
                teamToUpdate.SpeedBonusScore = team.SpeedBonusScore;
                teamToUpdate.SkillScore = team.SkillScore;
                teamToUpdate.TotalScore = team.TotalScore;
                teamToUpdate.Rank = team.Rank;
                teamToUpdate.JuniorRank = team.JuniorRank;
            }
        }

        public TeamData getTeam(int teamId)
        {
            return Teams.Find(team => team.TeamId == teamId);
        }

        public TeamData updateWithGateInfo(GateInformation gateInfo)
        {
            var teamToUpdate = Teams.Find(team => team.TeamId == gateInfo.TeamId);

            if (teamToUpdate != null)
            {
                teamToUpdate.SkillScore = gateInfo.TotalSkillScore;
            }

            return teamToUpdate;
        }

        public TeamData updateWithSpeedRaceScore(SpeedRaceScore speedRaceScore)
        {
            var teamToUpdate = Teams.Find(team => team.TeamId == speedRaceScore.TeamId);

            if (teamToUpdate != null)
            {
                teamToUpdate.SpeedTimes = speedRaceScore.SpeedTimes;
            }

            return teamToUpdate;
        }

        public TeamData updateWithSafetyCarFollowInfo(SafetyCarFollowInformation followInformation)
        {
            var teamToUpdate = Teams.Find(team => team.TeamId == followInformation.TeamId);

            if (teamToUpdate != null)
            {
                teamToUpdate.SafetyCarWasFollowed = followInformation.SafetyCarFollowed;
            }

            return teamToUpdate;
        }

        public TeamData updateWithSafetyCarOvertakeInfo(SafetyCarOvertakeInformation overtakeInformation)
        {
            var teamToUpdate = Teams.Find(team => team.TeamId == overtakeInformation.TeamId);

            if (teamToUpdate != null)
            {
                teamToUpdate.NumberOfOvertakes = overtakeInformation.NumberOfOvertakes;
            }

            return teamToUpdate;
        }
    }
}