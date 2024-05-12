using System.Collections.Generic;
using System.Windows;

namespace battleshipw3
{
    /// <summary>
    /// Interaction logic for LeaderboardWindow.xaml
    /// </summary>

    public partial class LeaderboardWindow : Window
        {
            public List<LeaderboardEntry> LeaderboardData { get; set; }

            public LeaderboardWindow(List<LeaderboardEntry> leaderboardData)
            {
                InitializeComponent();
                LeaderboardData = leaderboardData;
                DataContext = this;
            }
    }
}
