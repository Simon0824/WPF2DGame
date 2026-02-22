using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace App
{
    public partial class MainWindow : Window
    {
        public const int FOREST = 1;
        public const int GRASS = 2;
        public const int STONE = 3;
        public const int WATER = 4;
        public const int SET_WOOD = 5;
        public const int SET_STONE = 6;
        public const int TERRRAINS = 7;
        private int[,] map;
        private int mapWidth;
        private int mapHeight;
        private Image[,] MapImage;
        private BitmapImage[] terrainImage = new BitmapImage[TERRRAINS];
        private int maxWood = 0;
        private int maxStone = 0;
        private int playerXposition = 0;
        private int playerYposition = 0;
        private Image playerImage;
        private int woodAmount = 0;
        private int stoneAmount = 0;

        private Key direction = Key.None;
        public MainWindow()
        {
            InitializeComponent();
            LoadTerrainImages();
            playerImage = new Image
            {
                Width = 100,
                Height = 100
            };
            BitmapImage bmpPlayer = new BitmapImage(new Uri("player.png", UriKind.Relative));
            playerImage.Source = bmpPlayer;
        }

        private void LoadTerrainImages()
        {
            terrainImage[FOREST] = new BitmapImage(new Uri("forest.png", UriKind.Relative));
            terrainImage[GRASS] = new BitmapImage(new Uri("grass.png", UriKind.Relative));
            terrainImage[STONE] = new BitmapImage(new Uri("stone.png", UriKind.Relative));
            terrainImage[WATER] = new BitmapImage(new Uri("water.png", UriKind.Relative));
            terrainImage[SET_WOOD] = new BitmapImage(new Uri("wood_set.png", UriKind.Relative));
            terrainImage[SET_STONE] = new BitmapImage(new Uri("stone_set.png", UriKind.Relative));
        }

        private void PlayerPositionUpdate()
        {
            Grid.SetRow(playerImage, playerYposition);
            Grid.SetColumn(playerImage, playerXposition);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {

            int newX = playerXposition;
            int newY = playerYposition;
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            {
                direction = e.Key;
            }
            if (e.Key == Key.W) newY--;
            else if (e.Key == Key.S) newY++;
            else if (e.Key == Key.A) newX--;
            else if (e.Key == Key.D) newX++;
            if (newX >= 0 && newX < mapWidth && newY >= 0 && newY < mapHeight)
            {
                if (map[newY, newX] != WATER)
                {
                    playerXposition = newX;
                    playerYposition = newY;
                    PlayerPositionUpdate();
                }
            }

            if (e.Key == Key.B)
            {
                if (map[playerYposition, playerXposition] == FOREST)
                {
                    map[playerYposition, playerXposition] = GRASS;
                    MapImage[playerYposition, playerXposition].Source = terrainImage[GRASS];
                    woodAmount++;
                    WoodLabel.Content = "Drewno: " + woodAmount;
                }
                else if (map[playerYposition, playerXposition] == STONE)
                {
                    map[playerYposition, playerXposition] = GRASS;
                    MapImage[playerYposition, playerXposition].Source = terrainImage[GRASS];
                    stoneAmount++;
                    StoneLabel.Content = "Kamień: " + stoneAmount;
                }
                if (stoneAmount == maxStone && woodAmount == maxWood)
                {
                    MessageBox.Show("Zebrano wystarczającą ilość!");
                    Grid.Visibility = Visibility.Hidden;
                    MapGrid.Visibility = Visibility.Hidden;
                    Top_Panel.Visibility = Visibility.Hidden;
                    Panel_Wybor.Visibility = Visibility.Hidden;
                    Panel_Menu2.Visibility = Visibility.Hidden;
                    Panel_Menu.Visibility = Visibility.Visible;
                }
            }
            if (e.Key == Key.P)
            {
                if (woodAmount > 0)
                {
                    if (direction == Key.Up)
                    {
                        newY = playerYposition - 1;
                    }
                    else if (direction == Key.Down)
                    {
                        newY = playerYposition + 1;
                    }
                    else if (direction == Key.Left)
                    {
                        newX = playerXposition - 1;
                    }
                    else if (direction == Key.Right)
                    {
                        newX = playerXposition + 1;
                    }

                    if (newX >= 0 && newX < mapWidth && newY >= 0 && newY < mapHeight)
                    {
                        if (map[newY, newX] == FOREST || map[newY, newX] == STONE)
                        {
                            MessageBox.Show("Nie mozesz stawiać na skale lub na drzewie!");
                        }
                        else
                        {
                            map[newY, newX] = SET_WOOD;
                            MapImage[newY, newX].Source = terrainImage[SET_WOOD];
                            woodAmount--;
                            maxWood--;
                            WoodLabel.Content = "Drewno: " + woodAmount;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nie możesz budować poza mapę!");
                    }
                }
                else
                {
                    MessageBox.Show("Nie masz drewna!");
                }
            }


            if (e.Key == Key.K)
            {
                if (stoneAmount > 0)
                {
                    if (direction == Key.Up)
                    {
                        newY = playerYposition - 1;
                    }
                    else if (direction == Key.Down)
                    {
                        newY = playerYposition + 1;
                    }
                    else if (direction == Key.Left)
                    {
                        newX = playerXposition - 1;
                    }
                    else if (direction == Key.Right)
                    {
                        newX = playerXposition + 1;
                    }

                    if (newX >= 0 && newX < mapWidth && newY >= 0 && newY < mapHeight)
                    {
                        if (map[newY, newX] == FOREST || map[newY, newX] == STONE)
                        {
                            MessageBox.Show("Nie mozesz stawiać na skale lub na drzewie!");
                        }
                        else
                        {
                            map[newY, newX] = SET_STONE;
                            MapImage[newY, newX].Source = terrainImage[SET_STONE];
                            stoneAmount--;
                            maxStone--;
                            StoneLabel.Content = "Kamień: " + stoneAmount;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nie możesz budować poza mapę!");
                    }
                }
                else
                {
                    MessageBox.Show("Nie masz kamienia!");
                }
            }

        }




        private void Map1_Click(object sender, RoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Visible;
            MapGrid.Visibility = Visibility.Visible;
            Top_Panel.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            try
            {
                Random rnd = new Random();
                mapHeight = 5;
                mapWidth = 5;
                map = new int[mapHeight, mapWidth];
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        map[i, j] = rnd.Next(1, 5);
                    }
                }

                map[0, 0] = 1;
                File.Delete("map.txt");
                StreamWriter writer = new StreamWriter("map.txt", true);
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        writer.Write(map[i, j] + " ");
                    }
                    writer.WriteLine();
                }
                writer.Close();

                MapGrid.Children.Clear();
                MapGrid.RowDefinitions.Clear();
                MapGrid.ColumnDefinitions.Clear();

                for (int y = 0; y < mapHeight; y++)
                {
                    MapGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
                }
                for (int x = 0; x < mapWidth; x++)
                {
                    MapGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                }

                MapImage = new Image[mapHeight, mapWidth];
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        Image img = new Image
                        {
                            Width = 100,
                            Height = 100
                        };

                        int type = map[y, x];
                        if (type >= 1 && type < TERRRAINS)
                        {
                            img.Source = terrainImage[type];
                        }
                        else
                        {
                            img.Source = null;
                        }
                        Grid.SetRow(img, y);
                        Grid.SetColumn(img, x);
                        MapGrid.Children.Add(img);
                        MapImage[y, x] = img;
                    }
                }

                MapGrid.Children.Add(playerImage);
                Panel.SetZIndex(playerImage, 1);
                playerXposition = 0;
                playerYposition = 0;
                PlayerPositionUpdate();
                woodAmount = 0;
                stoneAmount = 0;
                maxWood = 0;
                maxStone = 0;
                WoodLabel.Content = "Drewno: " + woodAmount;
                StoneLabel.Content = "Kamień: " + stoneAmount;
                for (int i = 0; i < mapHeight; i++)
                {
                    for (int j = 0; j < mapWidth; j++)
                    {
                        if (map[i, j] == FOREST)
                            maxWood++;
                        else if (map[i, j] == STONE)
                            maxStone++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        private void Map2_Click(object sender, RoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Visible;
            MapGrid.Visibility = Visibility.Visible;
            Top_Panel.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            try
            {
                Random rnd = new Random();
                mapHeight = 6;
                mapWidth = 6;
                map = new int[mapHeight, mapWidth];
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        map[i, j] = rnd.Next(1, 5);
                    }
                }

                map[0, 0] = 1;
                File.Delete("map2.txt");
                StreamWriter writer = new StreamWriter("map2.txt", true);
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        writer.Write(map[i, j] + " ");
                    }
                    writer.WriteLine();
                }
                writer.Close();

                MapGrid.Children.Clear();
                MapGrid.RowDefinitions.Clear();
                MapGrid.ColumnDefinitions.Clear();

                for (int y = 0; y < mapHeight; y++)
                {
                    MapGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
                }
                for (int x = 0; x < mapWidth; x++)
                {
                    MapGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                }

                MapImage = new Image[mapHeight, mapWidth];
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        Image img = new Image
                        {
                            Width = 100,
                            Height = 100
                        };

                        int type = map[y, x];
                        if (type >= 1 && type < TERRRAINS)
                        {
                            img.Source = terrainImage[type];
                        }
                        else
                        {
                            img.Source = null;
                        }
                        Grid.SetRow(img, y);
                        Grid.SetColumn(img, x);
                        MapGrid.Children.Add(img);
                        MapImage[y, x] = img;
                    }
                }

                MapGrid.Children.Add(playerImage);
                Panel.SetZIndex(playerImage, 1);
                playerXposition = 0;
                playerYposition = 0;
                PlayerPositionUpdate();
                woodAmount = 0;
                stoneAmount = 0;
                maxWood = 0;
                maxStone = 0;
                WoodLabel.Content = "Drewno: " + woodAmount;
                StoneLabel.Content = "Kamień: " + stoneAmount;

                for (int i = 0; i < mapHeight; i++)
                {
                    for (int j = 0; j < mapWidth; j++)
                    {
                        if (map[i, j] == FOREST)
                            maxWood++;
                        else if (map[i, j] == STONE)
                            maxStone++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        private void Map3_Click(object sender, RoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Visible;
            MapGrid.Visibility = Visibility.Visible;
            Top_Panel.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            try
            {
                Random rnd = new Random();
                mapHeight = 8;
                mapWidth = 8;
                map = new int[mapHeight, mapWidth];
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        map[i, j] = rnd.Next(1, 5);
                    }
                }

                map[0, 0] = 1;
                File.Delete("map3.txt");
                StreamWriter writer = new StreamWriter("map3.txt", true);
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        writer.Write(map[i, j] + " ");
                    }
                    writer.WriteLine();
                }
                writer.Close();

                MapGrid.Children.Clear();
                MapGrid.RowDefinitions.Clear();
                MapGrid.ColumnDefinitions.Clear();

                for (int y = 0; y < mapHeight; y++)
                {
                    MapGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
                }
                for (int x = 0; x < mapWidth; x++)
                {
                    MapGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                }

                MapImage = new Image[mapHeight, mapWidth];
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        Image img = new Image
                        {
                            Width = 100,
                            Height = 100
                        };

                        int type = map[y, x];
                        if (type >= 1 && type < TERRRAINS)
                        {
                            img.Source = terrainImage[type];
                        }
                        else
                        {
                            img.Source = null;
                        }
                        Grid.SetRow(img, y);
                        Grid.SetColumn(img, x);
                        MapGrid.Children.Add(img);
                        MapImage[y, x] = img;
                    }
                }

                MapGrid.Children.Add(playerImage);
                Panel.SetZIndex(playerImage, 1);
                playerXposition = 0;
                playerYposition = 0;
                PlayerPositionUpdate();
                woodAmount = 0;
                stoneAmount = 0;
                maxWood = 0;
                maxStone = 0;
                WoodLabel.Content = "Drewno: " + woodAmount;
                StoneLabel.Content = "Kamień: " + stoneAmount;

                for (int i = 0; i < mapHeight; i++)
                {
                    for (int j = 0; j < mapWidth; j++)
                    {
                        if (map[i, j] == FOREST)
                            maxWood++;
                        else if (map[i, j] == STONE)
                            maxStone++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        private void Btn_Menu_Click(object sender, RoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Hidden;
            MapGrid.Visibility = Visibility.Hidden;
            Top_Panel.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Wybor.Visibility = Visibility.Visible;
            Panel_Sterowanie.Visibility = Visibility.Hidden;
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Visible;
            MapGrid.Visibility = Visibility.Visible;
            Top_Panel.Visibility = Visibility.Visible;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Sterowanie.Visibility = Visibility.Hidden;
        }

        private void Map_Change_Click(object sender, RoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Hidden;
            MapGrid.Visibility = Visibility.Hidden;
            Top_Panel.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Sterowanie.Visibility = Visibility.Hidden;
        }

        private void Moving_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aby się poruszać, posługuj się klawiszami WASD");
        }

        private void Put_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Klawisz `B`, aby zniszczyć drzewo/skałę. Użyj strzałek, aby wybrać kierunek. Klawiszem `P` postaw drewno a klawiszem `K` kamień");
        }

        private void Controll_Click(object sender, RoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Hidden;
            MapGrid.Visibility = Visibility.Hidden;
            Top_Panel.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Sterowanie.Visibility = Visibility.Visible;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Grid.Visibility = Visibility.Hidden;
            MapGrid.Visibility = Visibility.Hidden;
            Top_Panel.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Wybor.Visibility = Visibility.Visible;
            Panel_Sterowanie.Visibility = Visibility.Hidden;
        }
    }
}