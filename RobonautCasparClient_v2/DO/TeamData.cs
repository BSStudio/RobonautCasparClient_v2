using System.Collections.Generic;
using System.Linq;

namespace RobonautCasparClient_v2.DO
{
    public class TeamData
    {
        public int TeamId { get; set; }
        public int Year { get; set; }
        public TeamType TeamType { get; set; }
        public string TeamName { get; set; }
        public List<string> TeamMembers { get; set; }
        public string LogoPath { get; set; }
        public int TechnicalScore { get; set; }
        public int SpeedScore { get; set; }
        public int SpeedBonusScore { get; set; }
        public List<int> SpeedTimes { get; set; }
        public bool Follow { get; set; }
        public int Overtake { get; set; }
        public int Votes { get; set; }
        public int AudienceScore { get; set; }
        public int QualificationScore { get; set; }
        public int TotalScore { get; set; }
        public int Rank { get; set; }
        public int RankJunior { get; set; }
        
        public string TeamMembersString
        {
            get{
                string members = "";
                if (TeamMembers.Count != 0)
                {
                    members = TeamMembers[0].Trim();
                    bool first = true;
                    foreach (var member in TeamMembers)
                    {
                        if (!first)
                        {
                            members += ", " + member.Trim();
                        }
                        else
                        {
                            first = false;
                        }
                    }
                }

                return members;
            }
        }

        public int FastestTime
        {
            get
            {
                if (SpeedTimes.Count == 0)
                    return int.MaxValue;
                return SpeedTimes.Min();
            }
        }

        public int SumSpeedPoints
        {
            get
            {
                return SpeedScore + SpeedBonusScore;
            }
        }
    }
}