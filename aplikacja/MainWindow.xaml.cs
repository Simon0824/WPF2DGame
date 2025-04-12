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
        public const int LAS = 1; //teren lasu
        public const int LAKA = 2; //teren łąki
        public const int SKALA = 3; //teren skały
        public const int WODA = 4; //teren wody
        public const int POSTAWIONE_DREWNO = 5; //postawione drewno
        public const int POSTAWIONY_KAMIEN = 6; //psotawiony kamień
        public const int ILE_TERENOW = 7; //liczba terenów +1
        private int[,] mapa; //tablica dwuwymiarowa mapy
        private int szerokoscMapy;
        private int wysokoscMapy;
        private Image[,] tablicaTerenu; //tablica dwuwymiarowa odpowiadająca terenom
        private BitmapImage[] obrazyTerenu = new BitmapImage[ILE_TERENOW]; //tablica wszystkich terenów
        private int doceloweDrewno = 0; //drewno, które jest do zebrania
        private int docelowyKamien = 0; //kamień, który jest do zebrania
        //zmienne przechowujące pozycję gracza na osi x i y
        private int pozycjaGraczaX = 0; 
        private int pozycjaGraczaY = 0;
        private Image obrazGracza; 
        private int iloscDrewna = 0; //zebrane drewno
        private int iloscKamienia = 0; //zebrany kamień

        private Key wybranyKierunek = Key.None; //zmienna przechowywująca wybrany kierunek
        public MainWindow()
        {
            InitializeComponent();
            WczytajObrazyTerenu();
            obrazGracza = new Image //wczytanie obrazu gracza
            {
                Width = 100,
                Height = 100
            };
            BitmapImage bmpGracza = new BitmapImage(new Uri("gracz.png", UriKind.Relative)); //obrazek z gracz.png
            obrazGracza.Source = bmpGracza;
        }

        private void WczytajObrazyTerenu() //wczytanie wszystkich możliwych terenów
        {
            obrazyTerenu[LAS] = new BitmapImage(new Uri("las.png", UriKind.Relative));
            obrazyTerenu[LAKA] = new BitmapImage(new Uri("laka.png", UriKind.Relative));
            obrazyTerenu[SKALA] = new BitmapImage(new Uri("skala.png", UriKind.Relative));
            obrazyTerenu[WODA] = new BitmapImage(new Uri("woda.png", UriKind.Relative));
            obrazyTerenu[POSTAWIONE_DREWNO] = new BitmapImage(new Uri("postawione_drewno.png", UriKind.Relative));
            obrazyTerenu[POSTAWIONY_KAMIEN] = new BitmapImage(new Uri("postawiony_kamien.png", UriKind.Relative));
        }

        private void AktualizujPozycjeGracza() //zmiana pozycji
        {
            Grid.SetRow(obrazGracza, pozycjaGraczaY);
            Grid.SetColumn(obrazGracza, pozycjaGraczaX);
        }

        private void OknoGlowne_KeyDown(object sender, KeyEventArgs e)
        {
            //zmienne przechowujące pozycje gracza
            int nowyX = pozycjaGraczaX;
            int nowyY = pozycjaGraczaY;
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right) //nadpisanie zmiennej wybranyKierunek
            {
                wybranyKierunek = e.Key;
            }
            //przypisanie klawiszy ruchu
            if (e.Key == Key.W) nowyY--;
            else if (e.Key == Key.S) nowyY++;
            else if (e.Key == Key.A) nowyX--;
            else if (e.Key == Key.D) nowyX++;
            if (nowyX >= 0 && nowyX < szerokoscMapy && nowyY >= 0 && nowyY < wysokoscMapy) //gracz nie może wyjść poza mapę
            {
                if (mapa[nowyY, nowyX] != WODA) //gracz nie może wejść na pole z wodą
                {
                    pozycjaGraczaX = nowyX;
                    pozycjaGraczaY = nowyY;
                    AktualizujPozycjeGracza();
                }
            }

            if (e.Key == Key.B) //działanie niszczenia surowców
            {
                if (mapa[pozycjaGraczaY, pozycjaGraczaX] == LAS)
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = LAKA;
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[LAKA]; //zmiana zdjęcia terenu na łąkę
                    iloscDrewna++;
                    EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
                }
                else if (mapa[pozycjaGraczaY, pozycjaGraczaX] == SKALA)
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = LAKA;
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[LAKA];
                    iloscKamienia++;
                    EtykietaKamienia.Content = "Kamień: " + iloscKamienia;
                }
                if (iloscKamienia == docelowyKamien && iloscDrewna == doceloweDrewno) //jeśli gracz zebrał wszystkie możliwe surowce
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
            if (e.Key == Key.P) //działanie stawiania drewna
            {
                if (iloscDrewna > 0)
                {
                    //jeśli kliknięte są strzałki to ustawiają klierunek
                    if (wybranyKierunek == Key.Up)
                    {
                        nowyY = pozycjaGraczaY - 1;
                    }
                    else if (wybranyKierunek == Key.Down)
                    {
                        nowyY = pozycjaGraczaY + 1;
                    }
                    else if (wybranyKierunek == Key.Left)
                    {
                        nowyX = pozycjaGraczaX - 1;
                    }
                    else if (wybranyKierunek == Key.Right)
                    {
                        nowyX = pozycjaGraczaX + 1;
                    }

                    if (nowyX >= 0 && nowyX < szerokoscMapy && nowyY >= 0 && nowyY < wysokoscMapy) //nie można budować poza mapą
                    {
                        if (mapa[nowyY, nowyX] == LAS || mapa[nowyY, nowyX] == SKALA)
                        {
                            MessageBox.Show("Nie mozesz stawiać na skale lub na drzewie!");
                        }
                        else
                        { 
                            //jeśli kliknięty został P to kamień zostanie postawiony na wskazanym przez strzałkę polu
                            tablicaTerenu[nowyY, nowyX].Source = obrazyTerenu[POSTAWIONE_DREWNO]; //zmiana obrazu
                            iloscDrewna--;
                            doceloweDrewno--;
                            EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
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


            if (e.Key == Key.K) //działanie stawiania kamienia
            {
                if (iloscKamienia > 0)
                {
                    if (wybranyKierunek == Key.Up)
                    {
                        nowyY = pozycjaGraczaY - 1;
                    }
                    else if (wybranyKierunek == Key.Down)
                    {
                        nowyY = pozycjaGraczaY + 1;
                    }
                    else if (wybranyKierunek == Key.Left)
                    {
                        nowyX = pozycjaGraczaX - 1;
                    }
                    else if (wybranyKierunek == Key.Right)
                    {
                        nowyX = pozycjaGraczaX + 1;
                    }

                    if (nowyX >= 0 && nowyX < szerokoscMapy && nowyY >= 0 && nowyY < wysokoscMapy)
                    {
                        if (mapa[nowyY, nowyX] == LAS || mapa[nowyY, nowyX] == SKALA)
                        {
                            MessageBox.Show("Nie mozesz stawiać na skale lub na drzewie!");
                        }
                        else
                        {
                            tablicaTerenu[nowyY, nowyX].Source = obrazyTerenu[POSTAWIONY_KAMIEN];
                            iloscKamienia--;
                            docelowyKamien--;
                            EtykietaKamienia.Content = "Kamień: " + iloscKamienia;
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




        private void Mapa1_Click(object sender, RoutedEventArgs e) //generowanie małej mapy
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
                //rozmiar wybranej mapy
                wysokoscMapy = 5;
                szerokoscMapy = 5;
                mapa = new int[wysokoscMapy, szerokoscMapy]; //nowy rozmiar mapy
                for (int i = 0; i < mapa.GetLength(0); i++)
                {
                    for (int j = 0; j < mapa.GetLength(1); j++)
                    {
                        mapa[i, j] = rnd.Next(1, 5); //losowanie liczb w tablicy mapy pomiedzy liczbami przypisanymi do terenów
                    }
                }

                mapa[0, 0] = 1; //gracz zawsze startuje na łące
                File.Delete("mapa.txt"); //usunięcie starego pliku, aby nie był nadpisywany
                StreamWriter writer = new StreamWriter("mapa.txt", true); //wypełnianie pliku
                for (int i = 0; i < mapa.GetLength(0); i++)
                {
                    for (int j = 0; j < mapa.GetLength(1); j++)
                    {
                        writer.Write(mapa[i, j] + " ");
                    }
                    writer.WriteLine();
                }
                writer.Close();

                //czyszczenie siatki mapy
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

                tablicaTerenu = new Image[wysokoscMapy, szerokoscMapy]; //rozmiar tablicy terenów
                for (int y = 0; y < wysokoscMapy; y++) //wypełnienie mapy obrazami
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
                            obraz.Source = obrazyTerenu[rodzaj];
                        }
                        else
                        {
                            obraz.Source = null;
                        }
                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);
                        tablicaTerenu[y, x] = obraz;
                    }
                }

                SiatkaMapy.Children.Add(obrazGracza); //dodanie obrazu gracza
                Panel.SetZIndex(obrazGracza, 1);
                //gracz jest na początku mapy
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza(); //aktualizacja pozycji
                //początkowa ilość surowców
                iloscDrewna = 0;
                iloscKamienia = 0;
                doceloweDrewno = 0;
                docelowyKamien = 0;
                //aktualizacja etykiety pokazującej liczbę surowców
                EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
                EtykietaKamienia.Content = "Kamień: " + iloscKamienia;
                //ustalenie maksymalnej liczby zebranych surowców
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        private void Mapa2_Click(object sender, RoutedEventArgs e) //generowanie średniej mapy
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
                szerokoscMapy = 6;
                mapa = new int[wysokoscMapy, szerokoscMapy];
                for (int i = 0; i < mapa.GetLength(0); i++)
                {
                    for (int j = 0; j < mapa.GetLength(1); j++)
                    {
                        mapa[i, j] = rnd.Next(1, 5);
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
                            obraz.Source = obrazyTerenu[rodzaj];
                        }
                        else
                        {
                            obraz.Source = null;
                        }
                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);
                        tablicaTerenu[y, x] = obraz;
                    }
                }

                SiatkaMapy.Children.Add(obrazGracza);
                Panel.SetZIndex(obrazGracza, 1);
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();
                iloscDrewna = 0;
                iloscKamienia = 0;
                doceloweDrewno = 0;
                docelowyKamien = 0;
                EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
                EtykietaKamienia.Content = "Kamień: " + iloscKamienia;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        private void Mapa3_Click(object sender, RoutedEventArgs e) //generowanie dużej mapy
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
                szerokoscMapy = 8;
                mapa = new int[wysokoscMapy, szerokoscMapy];
                for (int i = 0; i < mapa.GetLength(0); i++)
                {
                    for (int j = 0; j < mapa.GetLength(1); j++)
                    {
                        mapa[i, j] = rnd.Next(1, 5);
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
                            obraz.Source = obrazyTerenu[rodzaj];
                        }
                        else
                        {
                            obraz.Source = null;
                        }
                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);
                        tablicaTerenu[y, x] = obraz;
                    }
                }

                SiatkaMapy.Children.Add(obrazGracza);
                Panel.SetZIndex(obrazGracza, 1);
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();
                iloscDrewna = 0;
                iloscKamienia = 0;
                doceloweDrewno = 0;
                docelowyKamien = 0;
                EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
                EtykietaKamienia.Content = "Kamień: " + iloscKamienia;

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
            }
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
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Wybor.Visibility = Visibility.Visible;
            Panel_Sterowanie.Visibility = Visibility.Hidden;
        }

        private void Wznow_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Visible;
            SiatkaMapy.Visibility = Visibility.Visible;
            Panel_gorny.Visibility = Visibility.Visible;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Sterowanie.Visibility = Visibility.Hidden;
        }

        private void Wybor_Mapy_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Hidden;
            SiatkaMapy.Visibility = Visibility.Hidden;
            Panel_gorny.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Visible;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Sterowanie.Visibility = Visibility.Hidden;
        }

        private void Poruszanie_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aby się poruszać, posługuj się klawiszami WASD");
        }

        private void Stawianie_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Klawisz `B`, aby zniszczyć drzewo/skałę. Użyj strzałek, aby wybrać kierunek. Klawiszem `P` postaw drewno a klawiszem `K` kamień");
        }

        private void Sterowanie_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Hidden;
            SiatkaMapy.Visibility = Visibility.Hidden;
            Panel_gorny.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Wybor.Visibility = Visibility.Hidden;
            Panel_Sterowanie.Visibility = Visibility.Visible;
        }

        private void Powracanie_Click(object sender, RoutedEventArgs e)
        {
            Siatka.Visibility = Visibility.Hidden;
            SiatkaMapy.Visibility = Visibility.Hidden;
            Panel_gorny.Visibility = Visibility.Hidden;
            Panel_Menu.Visibility = Visibility.Hidden;
            Panel_Menu2.Visibility = Visibility.Hidden;
            Panel_Wybor.Visibility = Visibility.Visible;
            Panel_Sterowanie.Visibility = Visibility.Hidden;
        }
    }
}