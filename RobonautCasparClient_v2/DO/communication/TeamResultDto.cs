using System.Collections.Generic;

namespace RobonautCasparClient_v2.DO.communication
{
    public class TeamResultDto
    {
        public int TeamId { get; set; }
        public int Year { get; set; }
        public int TechnicalScore { get; set; }
        public int SpeedScore { get; set; }
        public int speedBonusScore { get; set; }
        public List<int> speedTimes { get; set; }
        public int Votes { get; set; }
        public int AudienceScore { get; set; }
        public int QualificationScore { get; set; }
        public int TotalScore { get; set; }
        public int Rank { get; set; }
        public int RankJunior { get; set; }
    }
}