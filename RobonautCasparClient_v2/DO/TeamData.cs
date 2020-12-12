using System.Collections.Generic;
using System.Windows.Documents;

namespace RobonautCasparClient_v2.DO
{
    public class TeamData
    {
        public int TeamId { get; set; }
        public int Year { get; set; }
        public TeamType TeamType { get; set; }
        public string TeamName { get; set; }
        public List<string> TeamMembers { get; set; }
        public int TechnicalScore { get; set; }
        public int SpeedScore { get; set; }
        public int SpeedBonusScore { get; set; }
        public List<int> SpeedTimes { get; set; }
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

        public string SpeedTimesString
        {
            get
            {
                string times = "";
                if (SpeedTimes.Count != 0)
                {
                    times = Converters.TimeToString(SpeedTimes[0]);
                    bool first = true;
                    foreach (var time in SpeedTimes)
                    {
                        if (!first)
                        {
                            times += ", " + Converters.TimeToString(time);
                        }
                        else
                        {
                            first = false;
                        }
                    }
                }

                return times;
            }
        }
    }
}