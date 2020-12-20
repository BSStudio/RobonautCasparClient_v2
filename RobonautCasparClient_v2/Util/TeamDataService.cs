using System.Collections.Generic;
using System.Reflection.Emit;
using RobonautCasparClient_v2.DO.communication;

namespace RobonautCasparClient_v2.DO
{
    public class TeamDataService
    {
        private static TeamDataService instance;

        public static TeamDataService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TeamDataService();
                }

                return instance;
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
                teamToUpdate.SpeedScore = team.SpeedScore;
                teamToUpdate.SpeedTimes = team.SpeedTimes;
                teamToUpdate.SpeedBonusScore = team.SpeedBonusScore;
                teamToUpdate.TechnicalScore = team.TechnicalScore;
                teamToUpdate.TotalScore = team.TotalScore;
                teamToUpdate.Rank = team.Rank;
                teamToUpdate.RankJunior = team.RankJunior;
            }
        }

        public TeamData getTeam(int teamId)
        {
            return Teams.Find(team => team.TeamId == teamId);
        }

        public TeamData updateWithTechScore(TechnicalScoreDto techScore)
        {
            var teamToUpdate = Teams.Find(team => team.Year == techScore.Year && team.TeamId == techScore.TeamId);

            if (teamToUpdate != null)
            {
                teamToUpdate.TechnicalScore = techScore.TotalTechnicalScore;
            }

            return teamToUpdate;
        }

        public TeamData updateWithSafetyCarBonus(SafetyCarEventDto safetyCarEvent)
        {
            var teamToUpdate = Teams.Find(team => team.Year == safetyCarEvent.Year && team.TeamId == safetyCarEvent.TeamId);

            if (teamToUpdate != null)
            {
                teamToUpdate.Follow = safetyCarEvent.Follow;
                teamToUpdate.Overtake = safetyCarEvent.Overtake;
                teamToUpdate.SpeedBonusScore = safetyCarEvent.TotalSpeedBonus;
            }

            return teamToUpdate;
        }

        public TeamData updateWithTeamResult(TeamResultDto teamResult)
        {
            var teamToUpdate = Teams.Find(team => team.Year == teamResult.Year && team.TeamId == teamResult.TeamId);

            if (teamToUpdate != null)
            {
                teamToUpdate.TechnicalScore = teamResult.TechnicalScore;
                teamToUpdate.SpeedScore = teamResult.SpeedScore;
                teamToUpdate.SpeedTimes = teamResult.speedTimes;
                teamToUpdate.Follow = teamResult.Follow;
                teamToUpdate.Overtake = teamResult.Overtake;
                teamToUpdate.Votes = teamResult.Votes;
                teamToUpdate.AudienceScore = teamResult.AudienceScore;
                teamToUpdate.QualificationScore = teamResult.QualificationScore;
                teamToUpdate.TotalScore = teamResult.TotalScore;
                teamToUpdate.Rank = teamResult.Rank;
                teamToUpdate.RankJunior = teamResult.RankJunior;
            }

            return teamToUpdate;
        }
    }
}