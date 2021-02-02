namespace RobonautCasparClient_v2.DO
{
    public class TeamWithRanks
    {
        public TeamData TeamData { get; set; }
        public int Rank { get; set; }
        public int JuniorRank { get; set; }

        public TeamWithRanks(TeamData teamData, int rank, int juniorRank)
        {
            TeamData = teamData;
            Rank = rank;
            JuniorRank = juniorRank;
        }
    }
}