using System;

namespace battleshipw3
{
    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public int Moves { get; set; }
        public string Result { get; set; }
        public DateTime Date { get; set; }

        public LeaderboardEntry(int rank, int moves, string result, DateTime date)
        {
            Rank = rank;
            Moves = moves;
            Result = result;
            Date = date;
        }
    }
}
