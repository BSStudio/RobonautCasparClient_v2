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
        public int SkillScore { get; set; }
        public List<int> SpeedTimes { get; set; }
        public bool SafetyCarWasFollowed { get; set; }
        public int NumberOfOvertakes { get; set; }
        public int Votes { get; set; }
        public int AudienceScore { get; set; }
        public int QualificationScore { get; set; }
        public Score CombinedScore { get; set; }
        public Score JuniorScore { get; set; }
        
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

        public string SpeedTimesString
        {
            get
            {
                string speedTimesString = "";
                bool first = true;
                
                foreach (var time in SpeedTimes)
                {
                    if (!first)
                    {
                        speedTimesString += ", ";
                    }
                    
                    speedTimesString += Converters.TimeToString(time);
                    first = false;
                }

                return speedTimesString;
            }
        }

        public string FollowHumanRead
        {
            get
            {
                return Converters.boolToIgenNem(SafetyCarWasFollowed);
            }
        }

        public bool IsJunior
        {
            get
            {
                return TeamType == TeamType.JUNIOR;
            }
        }
    }
}