using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace battleshipw3
{
    public class LeaderboardManager
    {
        private  string LeaderboardFilePath {get;}

        public List<LeaderboardEntry> LoadLeaderboard()
        {
            if (File.Exists(LeaderboardFilePath))
            {
                string json = File.ReadAllText(LeaderboardFilePath);
                return JsonConvert.DeserializeObject<List<LeaderboardEntry>>(json);
            }

            // Returns an empty leaderboard if the file doesn't exist
            return new List<LeaderboardEntry>();
        }
        public LeaderboardManager(string filePath)
        {
            LeaderboardFilePath = filePath;
        }

        public void SaveLeaderboard(List<LeaderboardEntry> leaderboard)
        {
            string json = JsonConvert.SerializeObject(leaderboard);
            File.WriteAllText(LeaderboardFilePath, json);
        }

        public void CreateLeaderboardFile()
        {
            if (!File.Exists(LeaderboardFilePath))
            {
                File.WriteAllText(LeaderboardFilePath, string.Empty);
            }
        }
    }
}
