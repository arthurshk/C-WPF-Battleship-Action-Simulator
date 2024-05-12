using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace battleshipw3
{
    public class EnemyShips
    {
        Random rand = new Random();
        //needed to make our grid ships visible and the enemies not visible at start of game
        public void RandomizePlayerShipPlacement(Grid grid)
        {
            PlaceShipsRandomly(grid, Brushes.DeepPink);
        }
        //needed to make our grid ships visible and the enemies not visible at start of game
        public void RandomizeEnemyShipPlacement(Grid grid)
        {
            PlaceShipsRandomly(grid, Brushes.Wheat);
        }
        private void PlaceShipsRandomly(Grid grid, Brush shipColor)
        {
            int[] shipSizes = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 }; //sized ships

            foreach (int size in shipSizes)
            {
                bool isValid = false;
                List<int> shipPositions = new List<int>();
                bool isHorizontal = rand.Next(2) == 0; //half time will generate horizontal and half time will generate vertical ship a 0 or a 1 half the time

                while (!isValid) //while it is true
                {
                    shipPositions.Clear();

                    if (isHorizontal)
                    {
                        int startingIndex = rand.Next(0, grid.Children.Count - size); // leftmost position of the ship on the grid
                        int startingRow = Grid.GetRow((Button)grid.Children[startingIndex]); //row number of the leftmost part of ship
                        //add the whole ship in the for loop
                        for (int i = startingIndex; i < startingIndex + size; i++)
                        {
                            shipPositions.Add(i);
                        }

                        isValid = ValidateShipPositions(grid, shipPositions, isHorizontal, size)
                            && startingRow < grid.RowDefinitions.Count;
                    }
                    else
                    {
                        int startingIndex = rand.Next(0, grid.Children.Count - (size * grid.RowDefinitions.Count) + 1); //ensure that the ship can fit on the vertical plane of the grid
                        int startingCol = Grid.GetColumn((Button)grid.Children[startingIndex]); //determination of which column the ship will get placed onto

                        for (int i = startingIndex; i < startingIndex + (size * grid.RowDefinitions.Count); i += grid.RowDefinitions.Count) //going up by row iteration to ensure that all possible combinations are covered for vertical placement
                        {
                            shipPositions.Add(i);
                        }

                        isValid = ValidateShipPositions(grid, shipPositions, !isHorizontal, size)
                            && startingCol < grid.ColumnDefinitions.Count;
                    }
                }

                foreach (int position in shipPositions)
                {
                    Button button = (Button)grid.Children[position];
                    button.Background = shipColor;
                    CellState cellState = (CellState)button.Tag;
                    cellState.HasShip = true; //sets the state of put down as having a ship on there
                    button.Tag = cellState;
                }
            }
        }
        private bool ValidateShipPositions(Grid grid, List<int> positions, bool isHorizontal, int size)
        {
            foreach (int position in positions)
            {
                if (position >= 0 && position < grid.Children.Count)
                {
                    //gets the row and col of the placed ships 
                    int row = Grid.GetRow((Button)grid.Children[position]);
                    int col = Grid.GetColumn((Button)grid.Children[position]);

                    // Checks for occupation of adjacent cells to not place there 
                    if (IsCellOccupied(grid, row - 1, col) ||
                        IsCellOccupied(grid, row + 1, col) ||
                        IsCellOccupied(grid, row, col - 1) ||
                        IsCellOccupied(grid, row, col + 1) ||
                        IsCellOccupied(grid, row - 1, col - 1) ||
                        IsCellOccupied(grid, row - 1, col + 1) ||
                        IsCellOccupied(grid, row + 1, col - 1) ||
                        IsCellOccupied(grid, row + 1, col + 1) ||
                        IsCellOccupied(grid, row, col))
                    {
                        return false; //make isValid false to continue searching until a valid space is found
                    }

                    // Checks for overflow to other rows (for horizontal ships)
                    if (isHorizontal && (col + (size - 1)) % grid.ColumnDefinitions.Count < col % grid.ColumnDefinitions.Count) //compares the column index of the last tile in the ship, if it wraps around to the smaller column index then it has overflown
                    {
                        return false;
                    }

                    // Checks for overflow to other columns (for vertical ships)
                    if (!isHorizontal && (row + (size - 1) * grid.RowDefinitions.Count) >= grid.RowDefinitions.Count * grid.ColumnDefinitions.Count) // compares the row index of the last tile in the ship if it is more than the total number of tiles in grid then it has overflown
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private bool IsCellOccupied(Grid grid, int row, int col)
        {
            if (row < 0 || row >= grid.RowDefinitions.Count ||
                col < 0 || col >= grid.ColumnDefinitions.Count)
            {
                return false; //if out of bounds return false
            }

            Button button = (Button)grid.Children
                .Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col); //retrieval of button element at the cell position

            if (button != null)
            {
                CellState cellState = button.Tag as CellState;
                if (cellState != null && cellState.HasShip)
                {
                    return true; //if true cannot place there
                }
            }

            return false;
        }
    }
}