using System.Collections.Generic;

namespace RobonautCasparClient_v2.DO.communication
{
    public class SpeedRaceScore
    {
        public long TeamId { get; set; }
        public List<int> SpeedTimes { get; set; }
    }
}