using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Aplikacja
{
    public partial class MainWindow : Window
    {
        // Stałe reprezentujące rodzaje terenu 
        public const int LAS = 1;     // las
        public const int LAKA = 2;     // łąka
        public const int SKALA = 3;   // skały
        public const int ILE_TERENOW = 4;   // ile terenów
        // Mapa przechowywana jako tablica dwuwymiarowa int
        private int[,] mapa;
        private int szerokoscMapy;
        private int wysokoscMapy;
        // Dwuwymiarowa tablica kontrolek Image reprezentujących segmenty mapy
        private Image[,] tablicaTerenu;
        // Tablica obrazków terenu – indeks odpowiada rodzajowi terenu
        // Indeks 1: las, 2: łąka, 3: skały
        private BitmapImage[] obrazyTerenu = new BitmapImage[ILE_TERENOW];
        private int doceloweDrewno = 0;
        private int docelowyKamien = 0;
        private bool prawda;
        // Pozycja gracza na mapie
        private int pozycjaGraczaX = 0;
        private int pozycjaGraczaY = 0;
        // Obrazek reprezentujący gracza
        private Image obrazGracza;
        // Licznik zgromadzonego drewna
        private int iloscDrewna = 0;

        private int iloscKamienia = 0;
        public MainWindow()
        {
            InitializeComponent();
            WczytajObrazyTerenu();

            // Inicjalizacja obrazka gracza
            obrazGracza = new Image
            {
                Width = 100,
                Height = 100
            };
            BitmapImage bmpGracza = new BitmapImage(new Uri("gracz.png", UriKind.Relative));
            obrazGracza.Source = bmpGracza;

        }

        private void WczytajObrazyTerenu()
        {
            // Zakładamy, że tablica jest indeksowana od 0, ale używamy indeksów 1-3
            obrazyTerenu[LAS] = new BitmapImage(new Uri("las.png", UriKind.Relative));
            obrazyTerenu[LAKA] = new BitmapImage(new Uri("laka.png", UriKind.Relative));
            obrazyTerenu[SKALA] = new BitmapImage(new Uri("skala.png", UriKind.Relative));
        }

        // Wczytuje mapę z pliku tekstowego i dynamicznie tworzy tablicę kontrolek Image
        private void WczytajMape(string sciezkaPliku)
        {
            try
            {
                var linie = File.ReadAllLines(sciezkaPliku);//zwraca tablicę stringów, np. linie[0] to pierwsza linia pliku
                wysokoscMapy = linie.Length;
                szerokoscMapy = linie[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;//zwraca liczbę elementów w tablicy
                mapa = new int[wysokoscMapy, szerokoscMapy];

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    var czesci = linie[y].Split(' ', StringSplitOptions.RemoveEmptyEntries);//zwraca tablicę stringów np. czesci[0] to pierwszy element linii
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        mapa[y, x] = int.Parse(czesci[x]);//wczytanie mapy z pliku
                    }
                }

                // Przygotowanie kontenera SiatkaMapy – czyszczenie elementów i definicji wierszy/kolumn
                SiatkaMapy.Children.Clear();
                SiatkaMapy.RowDefinitions.Clear();
                SiatkaMapy.ColumnDefinitions.Clear();

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    SiatkaMapy.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
                }
                for (int x = 0; x < szerokoscMapy; x++)
                {
                    SiatkaMapy.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                }

                // Tworzenie tablicy kontrolk Image i dodawanie ich do siatki
                tablicaTerenu = new Image[wysokoscMapy, szerokoscMapy];
                for (int y = 0; y < wysokoscMapy; y++)
                {
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        Image obraz = new Image
                        {
                            Width = 100,
                            Height = 100
                        };

                        int rodzaj = mapa[y, x];
                        if (rodzaj >= 1 && rodzaj < ILE_TERENOW)
                        {
                            obraz.Source = obrazyTerenu[rodzaj];//wczytanie obrazka terenu
                        }
                        else
                        {
                            obraz.Source = null;
                        }
                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);//dodanie obrazka do siatki na ekranie
                        tablicaTerenu[y, x] = obraz;
                    }
                }

                // Dodanie obrazka gracza – ustawiamy go na wierzchu
                SiatkaMapy.Children.Add(obrazGracza);
                Panel.SetZIndex(obrazGracza, 1);//ustawienie obrazka gracza na wierzchu
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();
                SiatkaMapy.Width = szerokoscMapy;
                SiatkaMapy.Height = wysokoscMapy;
            }//koniec try
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        // Aktualizuje pozycję obrazka gracza w siatce
        private void AktualizujPozycjeGracza()
        {
            Grid.SetRow(obrazGracza, pozycjaGraczaY);
            Grid.SetColumn(obrazGracza, pozycjaGraczaX);
        }

        // Obsługa naciśnięć klawiszy – ruch gracza oraz wycinanie lasu
        private void OknoGlowne_KeyDown(object sender, KeyEventArgs e)
        {
            int nowyX = pozycjaGraczaX;
            int nowyY = pozycjaGraczaY;
            //zmiana pozycji gracza
            if (e.Key == Key.W) nowyY--;
            else if (e.Key == Key.S) nowyY++;
            else if (e.Key == Key.A) nowyX--;
            else if (e.Key == Key.D) nowyX++;
            //Gracz nie może wyjść poza mapę
            if (nowyX >= 0 && nowyX < szerokoscMapy && nowyY >= 0 && nowyY < wysokoscMapy)
            {
                    pozycjaGraczaX = nowyX;
                    pozycjaGraczaY = nowyY;
                    AktualizujPozycjeGracza();
            }

            if (e.Key == Key.B)
            {
                if (mapa[pozycjaGraczaY, pozycjaGraczaX] == LAS)//jeśli gracz stoi na polu lasu
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = LAKA;
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[LAKA];
                    iloscDrewna++;
                    EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
                }
                else if (mapa[pozycjaGraczaY, pozycjaGraczaX] == SKALA)//jeśli gracz stoi na polu lasu
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = LAKA;
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[LAKA];
                    iloscKamienia++;
                    EtykietaKamienia.Content = "Kamień: " + iloscKamienia;
                }
                if (iloscKamienia == docelowyKamien && iloscDrewna == doceloweDrewno)
                {
                    MessageBox.Show("Zebrano wystarczającą ilość!");
                    Siatka.Visibility = Visibility.Hidden;
                    SiatkaMapy.Visibility = Visibility.Hidden;
                    Panel_gorny.Visibility = Visibility.Hidden;
                    Panel_Wybor.Visibility = Visibility.Hidden;
                    Panel_Menu2.Visibility = Visibility.Hidden;
                    Panel_Menu.Visibility = Visibility.Visible;
                }
            }
        }

        private void Mapa1_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Visible;
            SiatkaMapy.Visibility = Visibility.Visible;
            Panel_gorny.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            try
            {
                Random rnd = new Random();
                wysokoscMapy = 5;
                szerokoscMapy = 5;//zwraca liczbę elementów w tablicy
                mapa = new int[wysokoscMapy, szerokoscMapy];
                for (int i = 0; i < mapa.GetLength(0); i++)
                {
                    for(int j = 0; j < mapa.GetLength(1); j ++)
                    {
                        mapa[i, j] = rnd.Next(1, 4);
                    }
                }

                mapa[0, 0] = 1;
                File.Delete("mapa.txt");
                StreamWriter writer = new StreamWriter("mapa.txt", true);
                for(int i = 0;i < mapa.GetLength(0);i++)
                {
                    for(int j = 0;j < mapa.GetLength(1);j ++)
                    {
                        writer.Write(mapa[i, j] + " ");
                    }
                    writer.WriteLine();
                }
                writer.Close();

                // Przygotowanie kontenera SiatkaMapy – czyszczenie elementów i definicji wierszy/kolumn
                SiatkaMapy.Children.Clear();
                SiatkaMapy.RowDefinitions.Clear();
                SiatkaMapy.ColumnDefinitions.Clear();

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    SiatkaMapy.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
                }
                for (int x = 0; x < szerokoscMapy; x++)
                {
                    SiatkaMapy.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                }

                // Tworzenie tablicy kontrolk Image i dodawanie ich do siatki
                tablicaTerenu = new Image[wysokoscMapy, szerokoscMapy];
                for (int y = 0; y < wysokoscMapy; y++)
                {
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        Image obraz = new Image
                        {
                            Width = 100,
                            Height = 100
                        };

                        int rodzaj = mapa[y, x];
                        if (rodzaj >= 1 && rodzaj < ILE_TERENOW)
                        {
                            obraz.Source = obrazyTerenu[rodzaj];//wczytanie obrazka terenu
                        }
                        else
                        {
                            obraz.Source = null;
                        }
                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);//dodanie obrazka do siatki na ekranie
                        tablicaTerenu[y, x] = obraz;
                    }
                }

                // Dodanie obrazka gracza – ustawiamy go na wierzchu
                SiatkaMapy.Children.Add(obrazGracza);
                Panel.SetZIndex(obrazGracza, 1);//ustawienie obrazka gracza na wierzchu
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();

                iloscDrewna = 0;
                EtykietaDrewna.Content = "Drewno: " + iloscDrewna;

                for (int i = 0; i < wysokoscMapy; i++)
                {
                    for (int j = 0; j < szerokoscMapy; j++)
                    {
                        if (mapa[i, j] == LAS)
                            doceloweDrewno++;
                        else if (mapa[i, j] == SKALA)
                            docelowyKamien++;
                    }
                }
            }//koniec try
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        private void Mapa2_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Visible;
            SiatkaMapy.Visibility = Visibility.Visible;
            Panel_gorny.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            try
            {
                Random rnd = new Random();
                wysokoscMapy = 6;
                szerokoscMapy = 6;//zwraca liczbę elementów w tablicy
                mapa = new int[wysokoscMapy, szerokoscMapy];
                for (int i = 0; i < mapa.GetLength(0); i++)
                {
                    for (int j = 0; j < mapa.GetLength(1); j++)
                    {
                        mapa[i, j] = rnd.Next(1, 4);
                    }
                }

                mapa[0, 0] = 1;
                File.Delete("mapa2.txt");
                StreamWriter writer = new StreamWriter("mapa2.txt", true);
                for (int i = 0; i < mapa.GetLength(0); i++)
                {
                    for (int j = 0; j < mapa.GetLength(1); j++)
                    {
                        writer.Write(mapa[i, j] + " ");
                    }
                    writer.WriteLine();
                }
                writer.Close();

                // Przygotowanie kontenera SiatkaMapy – czyszczenie elementów i definicji wierszy/kolumn
                SiatkaMapy.Children.Clear();
                SiatkaMapy.RowDefinitions.Clear();
                SiatkaMapy.ColumnDefinitions.Clear();

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    SiatkaMapy.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
                }
                for (int x = 0; x < szerokoscMapy; x++)
                {
                    SiatkaMapy.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                }

                // Tworzenie tablicy kontrolk Image i dodawanie ich do siatki
                tablicaTerenu = new Image[wysokoscMapy, szerokoscMapy];
                for (int y = 0; y < wysokoscMapy; y++)
                {
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        Image obraz = new Image
                        {
                            Width = 100,
                            Height = 100
                        };

                        int rodzaj = mapa[y, x];
                        if (rodzaj >= 1 && rodzaj < ILE_TERENOW)
                        {
                            obraz.Source = obrazyTerenu[rodzaj];//wczytanie obrazka terenu
                        }
                        else
                        {
                            obraz.Source = null;
                        }
                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);//dodanie obrazka do siatki na ekranie
                        tablicaTerenu[y, x] = obraz;
                    }
                }

                // Dodanie obrazka gracza – ustawiamy go na wierzchu
                SiatkaMapy.Children.Add(obrazGracza);
                Panel.SetZIndex(obrazGracza, 1);//ustawienie obrazka gracza na wierzchu
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();
                for (int i = 0; i < wysokoscMapy; i++)
                {
                    for (int j = 0; j < szerokoscMapy; j++)
                    {
                        if (mapa[i, j] == LAS)
                            doceloweDrewno++;
                        else if (mapa[i, j] == SKALA)
                            docelowyKamien++;
                    }
                }
            }//koniec try
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        private void Mapa3_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Visible;
            SiatkaMapy.Visibility = Visibility.Visible;
            Panel_gorny.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            try
            {
                Random rnd = new Random();
                wysokoscMapy = 8;
                szerokoscMapy = 8;//zwraca liczbę elementów w tablicy
                mapa = new int[wysokoscMapy, szerokoscMapy];
                for (int i = 0; i < mapa.GetLength(0); i++)
                {
                    for (int j = 0; j < mapa.GetLength(1); j++)
                    {
                        mapa[i, j] = rnd.Next(1, 4);
                    }
                }

                mapa[0, 0] = 1;
                File.Delete("mapa3.txt");
                StreamWriter writer = new StreamWriter("mapa3.txt", true);
                for (int i = 0; i < mapa.GetLength(0); i++)
                {
                    for (int j = 0; j < mapa.GetLength(1); j++)
                    {
                        writer.Write(mapa[i, j] + " ");
                    }
                    writer.WriteLine();
                }
                writer.Close();

                // Przygotowanie kontenera SiatkaMapy – czyszczenie elementów i definicji wierszy/kolumn
                SiatkaMapy.Children.Clear();
                SiatkaMapy.RowDefinitions.Clear();
                SiatkaMapy.ColumnDefinitions.Clear();

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    SiatkaMapy.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
                }
                for (int x = 0; x < szerokoscMapy; x++)
                {
                    SiatkaMapy.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                }

                // Tworzenie tablicy kontrolk Image i dodawanie ich do siatki
                tablicaTerenu = new Image[wysokoscMapy, szerokoscMapy];
                for (int y = 0; y < wysokoscMapy; y++)
                {
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        Image obraz = new Image
                        {
                            Width = 100,
                            Height = 100
                        };

                        int rodzaj = mapa[y, x];
                        if (rodzaj >= 1 && rodzaj < ILE_TERENOW)
                        {
                            obraz.Source = obrazyTerenu[rodzaj];//wczytanie obrazka terenu
                        }
                        else
                        {
                            obraz.Source = null;
                        }
                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);//dodanie obrazka do siatki na ekranie
                        tablicaTerenu[y, x] = obraz;
                    }
                }

                // Dodanie obrazka gracza – ustawiamy go na wierzchu
                SiatkaMapy.Children.Add(obrazGracza);
                Panel.SetZIndex(obrazGracza, 1);//ustawienie obrazka gracza na wierzchu
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();
                for (int i = 0; i < wysokoscMapy; i++)
                {
                    for (int j = 0; j < szerokoscMapy; j++)
                    {
                        if (mapa[i, j] == LAS)
                            doceloweDrewno++;
                        else if (mapa[i, j] == SKALA)
                            docelowyKamien++;
                    }
                }
            }//koniec try
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        private void Btn_Menu_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Hidden;
            SiatkaMapy.Visibility = Visibility.Hidden;
            Panel_gorny.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
        }

        private void Wznow_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Visible;
            SiatkaMapy.Visibility = Visibility.Visible;
            Panel_gorny.Visibility = Visibility.Visible;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Wybor.Visibility = Visibility.Hidden;
        }

        private void Wybor_Mapy_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Hidden;
            SiatkaMapy.Visibility = Visibility.Hidden;
            Panel_gorny.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
        }
    }
}