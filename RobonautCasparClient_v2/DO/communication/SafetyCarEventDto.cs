namespace RobonautCasparClient_v2.DO.communication
{
    public class SafetyCarEventDto
    {
        public int Year { get; set; }
        public int TeamId { get; set; }
        public bool Follow { get; set; }
        public int Overtake { get; set; }
        public int Score { get; set; }
        public int TotalSpeedBonus { get; set; }
    }
}