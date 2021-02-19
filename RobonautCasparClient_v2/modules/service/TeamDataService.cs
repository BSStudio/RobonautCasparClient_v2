using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
                teamToUpdate.SpeedTimes = team.SpeedTimes;
                teamToUpdate.SkillScore = team.SkillScore;
                teamToUpdate.JuniorScore = team.JuniorScore;
                teamToUpdate.CombinedScore = team.CombinedScore;
            }
        }

        public TeamData getTeam(int teamId)
        {
            return Teams.Find(team => team.TeamId == teamId);
        }

        public List<TeamWithRanks> getTeamsWithRanks()
        {
            List<TeamWithRanks> ranksList = new List<TeamWithRanks>();

            Teams.Sort((a, b) =>
            {
                int result = b.CombinedScore.TotalScore - a.CombinedScore.TotalScore;

                return result != 0 ? result : a.FastestTime - b.FastestTime;
            });

            var rank = 0;
            var rankJump = 1;
            var lastScore = -1;
            var lastBestTime = -1;

            var juniorRank = 0;
            var juniorLastScore = -1;

            foreach (var team in Teams)
            {
                if (lastScore != team.CombinedScore.TotalScore)
                {
                    rank++;
                    lastScore = team.CombinedScore.TotalScore;
                }
                else if (lastBestTime != team.FastestTime)
                {
                    rank++;
                }

                if (team.TeamType == TeamType.JUNIOR)
                {
                    if (juniorLastScore != team.JuniorScore.TotalScore)
                    {
                        juniorRank++;
                        juniorLastScore = team.JuniorScore.TotalScore;
                    }
                    else if (lastBestTime != team.FastestTime)
                    {
                        juniorRank++;
                    }

                }
                
                lastBestTime = team.FastestTime;

                ranksList.Add(new TeamWithRanks(team, rank, team.TeamType == TeamType.JUNIOR ? juniorRank : -1));
            }

            return ranksList;
        }

        public List<(int rank, TeamData teamData)> getTeamsWithSpecialRank(FullScreenTableType type)
        {
            List<(int rank, TeamData teamData)> rankList = new List<(int rank, TeamData teamData)>();

            var rank = 0;
            var lastScore = -1;
            int lastBestTime = -1;

            switch (type)
            {
                case FullScreenTableType.QUALIFICATION_POINTS:
                    Teams.Sort((a, b) => b.QualificationScore - a.QualificationScore);

                    foreach (var team in Teams)
                    {
                        if (lastScore != team.QualificationScore)
                        {
                            rank++;
                            lastScore = team.QualificationScore;
                        }

                        rankList.Add((rank, team));
                    }

                    break;
                case FullScreenTableType.AUDIENCE_POINTS:
                    Teams.Sort((a, b) => b.Votes - a.Votes);

                    foreach (var team in Teams)
                    {
                        if (lastScore != team.Votes)
                        {
                            rank++;
                            lastScore = team.Votes;
                        }

                        rankList.Add((rank, team));
                    }

                    break;
                case FullScreenTableType.SKILL_POINTS:
                    Teams.Sort((a, b) => b.SkillScore - a.SkillScore);

                    foreach (var team in Teams)
                    {
                        if (lastScore != team.SkillScore)
                        {
                            rank++;
                            lastScore = team.SkillScore;
                        }

                        rankList.Add((rank, team));
                    }

                    break;
                case FullScreenTableType.SPEED_TIMES:
                    Teams.Sort((a, b) => a.FastestTime - b.FastestTime);

                    foreach (var team in Teams)
                    {
                        if (lastScore != team.FastestTime)
                        {
                            rank++;
                            lastScore = team.FastestTime;
                        }

                        rankList.Add((rank, team));
                    }

                    break;
                case FullScreenTableType.SPEED_POINTS:
                    Teams.Sort((a, b) => b.CombinedScore.SpeedScore - a.CombinedScore.SpeedScore);

                    foreach (var team in Teams)
                    {
                        if (lastScore != team.CombinedScore.SpeedScore)
                        {
                            rank++;
                            lastScore = team.CombinedScore.SpeedScore;
                        }

                        rankList.Add((rank, team));
                    }

                    break;
                case FullScreenTableType.FINAL_JUNIOR:
                    var teams = Teams.Where(team => team.TeamType == TeamType.JUNIOR).ToList();
                    teams.Sort((a, b) =>
                    {
                        int result = b.JuniorScore.TotalScore - a.JuniorScore.TotalScore;

                        return result != 0 ? result : a.FastestTime - b.FastestTime;
                    });

                    foreach (var team in teams)
                    {
                        if (lastScore != team.JuniorScore.TotalScore)
                        {
                            rank++;
                            lastScore = team.JuniorScore.TotalScore;
                        }
                        else if (lastBestTime != team.FastestTime)
                        {
                            rank++;
                        }

                        lastBestTime = team.FastestTime;

                        rankList.Add((rank, team));
                    }

                    break;
                case FullScreenTableType.FINAL:
                    Teams.Sort((a, b) =>
                    {
                        int result = b.CombinedScore.TotalScore - a.CombinedScore.TotalScore;

                        return result != 0 ? result : a.FastestTime - b.FastestTime;
                    });
                    
                    foreach (var team in Teams)
                    {
                        if (lastScore != team.CombinedScore.TotalScore)
                        {
                            rank++;
                            lastScore = team.CombinedScore.TotalScore;
                        }
                        else if (lastBestTime != team.FastestTime)
                        {
                            rank++;
                        }

                        lastBestTime = team.FastestTime;

                        rankList.Add((rank, team));
                    }

                    break;
            }

            return rankList;
        }

        public int getTeamRank(TeamData team, TeamType teamType)
        {
            List<TeamData> currentTeams = new List<TeamData>();

            if (teamType == TeamType.JUNIOR)
            {
                currentTeams = Teams
                    .Where(teamD => teamD.TeamType == TeamType.JUNIOR)
                    .ToList();

                currentTeams.Sort((a, b) =>
                {
                    int result = b.JuniorScore.TotalScore - a.JuniorScore.TotalScore;

                    return result != 0 ? result : a.FastestTime - b.FastestTime;
                });
            }
            else
            {
                currentTeams = Teams;

                currentTeams.Sort((a, b) =>
                {
                    int result = b.CombinedScore.TotalScore - a.CombinedScore.TotalScore;

                    return result != 0 ? result : a.FastestTime - b.FastestTime;
                });
            }

            var rank = 0;
            var lastScore = -1;
            var lastBestTime = -1;

            foreach (var _team in currentTeams)
            {
                var currentScore = teamType == TeamType.JUNIOR
                    ? _team.JuniorScore.TotalScore
                    : _team.CombinedScore.TotalScore;

                if (lastScore != currentScore)
                {
                    rank += 1;
                    lastScore = currentScore;
                }
                else if (lastBestTime != team.FastestTime)
                {
                    rank++;
                }

                lastBestTime = _team.FastestTime;

                if (team.TeamId == _team.TeamId && team.Year == _team.Year)
                {
                    return rank;
                }
            }

            return -1;
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