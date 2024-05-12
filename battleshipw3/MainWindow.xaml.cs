using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace battleshipw3
{
    public partial class MainWindow : Window
    {
        private int[,] MyArrayPlace;
        private int[,] EnemyArrayPlace;
        EnemyShips enemyShips;
        private readonly int row = 10;
        private readonly int col = 10;
        Random random;
        private int movesCount = 0;
        LeaderboardManager leaderboardManager;
        //leaderboard file we are importing the data of game to
        string leaderboardFilePath = "leaderboard.json";
        public MainWindow()
        {
            InitializeComponent();
            InitializationArray();
            enemyShips = new EnemyShips();
            random = new Random();
            enemyShips.RandomizePlayerShipPlacement(gridMe);
            enemyShips.RandomizeEnemyShipPlacement(gridEnemy);
            leaderboardManager = new LeaderboardManager(leaderboardFilePath); //object initialization
            leaderboardManager.CreateLeaderboardFile();
        }
        private void GameOver(bool playerWon)
        {
            // Loads leaderboard data
            LeaderboardManager leaderboardManager = new LeaderboardManager(leaderboardFilePath);
            List<LeaderboardEntry> leaderboardData = leaderboardManager.LoadLeaderboard();
            // Initializes the leaderboardData list if it is null
            if (leaderboardData == null)
            {
                leaderboardData = new List<LeaderboardEntry>();
            }
            // Creates a new leaderboard entry
            LeaderboardEntry entry = new LeaderboardEntry(1, movesCount, playerWon ? "Won" : "Lost", DateTime.Now);
            // Adds the entry to the leaderboard data
            leaderboardData.Add(entry);
            // Sorts the leaderboard data on made moves in ascending order
            leaderboardData = leaderboardData.OrderBy(e => e.Moves).ToList();
            // Assigns ranks based on the sorted order
            for (int i = 0; i < leaderboardData.Count; i++)
            {
                leaderboardData[i].Rank = i + 1;
            }
            // Saving the updated leaderboard data
            leaderboardManager.SaveLeaderboard(leaderboardData);
            // Showing the leaderboard window
            LeaderboardWindow leaderboardWindow = new LeaderboardWindow(leaderboardData);
            leaderboardWindow.ShowDialog();
        }
        private void InitializationArray()
        {
            //used to create/add the maps buttons for the entire array of the board 
            MyArrayPlace = new int[row, col];
            EnemyArrayPlace = new int[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Button buttonMe = new Button() { Content = "Click", Background = Brushes.Wheat, FontSize = 7, Foreground = Brushes.BlueViolet };
                    gridMe.Children.Add(buttonMe);
                    Grid.SetRow(buttonMe, i);
                    Grid.SetColumn(buttonMe, j);
                
                    CellState playerCellState = new CellState();
                    buttonMe.Tag = playerCellState;

                    Button buttonEnemy = new Button() { Content = "Click", Background = Brushes.Wheat, FontSize = 7, Foreground = Brushes.BlueViolet };
                    gridEnemy.Children.Add(buttonEnemy);
                    Grid.SetRow(buttonEnemy, i);
                    Grid.SetColumn(buttonEnemy, j);
                    buttonEnemy.Click += ButtonEnemy_Click;
                    //initialize the cells to false as not having a ship at first
                    CellState enemyCellState = new CellState();
                    buttonEnemy.Tag = enemyCellState;
                }
            }
        }
        private HashSet<(int, int)> firedSquares = new HashSet<(int, int)>(); //track which squares on enemy board have been fired at to prevent refiring same square
        private void ButtonEnemy_Click(object sender, RoutedEventArgs e)
        {
            int row = Grid.GetRow((Button)sender);
            int col = Grid.GetColumn((Button)sender);
            if (firedSquares.Contains((row, col)))
            {
                MessageBox.Show("You have already fired at this square. Choose a different square.");
                return;
            }

            firedSquares.Add((row, col)); //adds on to our hash set for future reference 
            movesCount++;
            CellState cellState = (CellState)((Button)sender).Tag;
            if (cellState.HasShip)
            {
                ((Button)sender).Background = Brushes.Red;
            }
            else
            {
                ((Button)sender).Background = Brushes.Blue;
            }
            //checks for game win/ game loss
            checkOurWin();
            shotToGridMe();
        }
        private void checkOurWin()
        {
            int sunkShipCount = 0;

            foreach (Button button in gridEnemy.Children)
            {
                CellState cellState = (CellState)button.Tag;
                if (cellState.HasShip && button.Background == Brushes.Red)
                {
                    sunkShipCount++;
                }
            }
            //when we've sunk 20 square containing ships we've won!
            //use of threads to decouple and show both the message box and the leaderboard at same time
            if (sunkShipCount == 20) 
            {
                Thread messageBoxThread = new Thread(() =>
                {
                    MessageBox.Show("You have won, congratulations!");
                });
                messageBoxThread.Start();
                GameOver(true); //initializes the creation of leaderboard
            }
        }
        private void checkEnemyWin()
        {
            int sunkShipCount = 0;
            //same logic as for checking our win, just shows a loss for when the enemy wins
            foreach (Button button in gridMe.Children)
            {
                CellState cellState = (CellState)button.Tag;
                if (cellState.HasShip && button.Background == Brushes.Red)
                {
                    sunkShipCount++;
                }
            }

            if (sunkShipCount == 20)
            {
                Thread messageBoxThread = new Thread(() =>
                {
                    MessageBox.Show("The enemy has won! Better luck next time.");
                });
                messageBoxThread.Start();

                GameOver(false);
            }
        }
        private List<int> firedPositions = new List<int>(); // Tracker for fired positions
        private void shotToGridMe()
        {
            int x = random.Next(0, 100);
            // Finds a random position that hasn't been fired upon
            while (firedPositions.Contains(x) ||
                   gridMe.Children.OfType<Button>().Any(but => but.Background != Brushes.Red && but.Background != Brushes.Blue && but.Background != Brushes.Wheat && but.Background != Brushes.DeepPink))
            {
                x = random.Next(0, 100);
            }
            firedPositions.Add(x); // Adds the fired position to the list
            // Updates the button background based on the hit or miss
            if (((Button)gridMe.Children[x]).Background == Brushes.DeepPink || ((Button)gridMe.Children[x]).Background == Brushes.Red)
            {
                ((Button)gridMe.Children[x]).Background = Brushes.Red;
            }
            else
            {
                ((Button)gridMe.Children[x]).Background = Brushes.Blue;
            }
            checkEnemyWin(); // Check if the enemy has won
            checkOurWin(); // Check if the player has won
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            instructionsTextblock.Foreground = Brushes.BlueViolet;
            instructionsTextblock.FontSize = 18;
            instructionsTextblock.Text = "Welcome to Battleships! Fire at the enemy board by clicking on any square under Enemy Board. Good Luck! \n Key: \n Placed Ship = Pink \n Hit = Red \n Miss = Blue";
        }
    }
}