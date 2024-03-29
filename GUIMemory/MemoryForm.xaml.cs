using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BusinessMemory;

namespace GUIMemory
{
    /// <summary>
    /// Interaction logic for MemoryForm.xaml
    /// </summary>
    public partial class MemoryForm : Window
    {
        MemoryCard<BitmapImage>[][] Grid { get; }
        private int _attempts;
        public int Attempts
        {
            get { return _attempts; }
            set
            {
                if (_attempts != value)
                {
                    _attempts = value;

                    // Update label when Attempts is set
                    Attempts_Label.Content = $"Attempts: {_attempts}";
                }
            }
        }

        private int _amountOfCards;
        public int AmountOfCards
        {
            get { return _amountOfCards; }
            set
            {
                if (_amountOfCards != value)
                {
                    _amountOfCards = value;

                    // Update label when AmountOfCards is set
                    Cards_Label.Content = $"Cards: {_amountOfCards}";
                }
            }
        }

        //interval van 1 seconde
        public Timer MemoryTimer = new Timer(1000);
        private int _seconds;
        public int Seconds
        {
            get { return _seconds; }
            set
            {
                if (_seconds != value)
                {
                    _seconds = value;

                    // Update label when Seconds is set
                    Timer_Label.Content = $"Time: {_seconds} seconds";
                }
            }
        }

        private int _cardsGuessed;
        public int CardsGuessed
        {
            get { return _cardsGuessed; }
            set
            {
                if(value == AmountOfCards)
                {
                    EndGame();
                    _cardsGuessed = value;
                }
                _cardsGuessed = value;
            }
        }

        private void EndGame()
        {
            MemoryTimer.Stop();
            DateTime date = DateTime.Now;
            MessageBox.Show($"You win with this score: {Math.Round(CalculateScore(),2)}");
            
            TableRankings form = new TableRankings(AmountOfCards, Seconds, Attempts, date);
            form.Show();
            this.Close();
        }

        private double CalculateScore()
        {
            return GameRules.CalculatePoints(AmountOfCards, Seconds, Attempts);
        }

        public MemoryForm(MemoryCard<BitmapImage>[][] grid)
        {
            Grid = grid;
            WindowState = WindowState.Maximized;
            InitializeComponent();
            Attempts = 0;
            AmountOfCards = MemoryCard<BitmapImage>.GetAmountOfOriginalCards(Grid)*2;
            Seconds = 0;

            // Stel de rij- en kolomdefinities in op basis van de grootte van je knoppenlijst
            for (int i = 0; i < Grid.Length; i++)
            {
                MemoryGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < Grid[0].Length; i++)
            {
                MemoryGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Voeg de knoppen toe aan het raster
            for (int row = 0; row < Grid.Length; row++)
            {
                for (int column = 0; column < Grid[row].Length; column++)
                {
                    if (Grid[row][column] == null) { continue; }
                    MemoryButton button = new MemoryButton(Grid[row][column]);
                    MemoryButton.Form = this;
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black; // Set the color of the border
                    border.BorderThickness = new Thickness(2); // Set the thickness of the border
                    border.Margin = new Thickness(3);
                    border.Child = button; // Set the MemoryButton as the child of the Border

                    System.Windows.Controls.Grid.SetRow(border, row);
                    System.Windows.Controls.Grid.SetColumn(border, column);

                    MemoryGrid.Children.Add(border);
                    
                }
            }

            MemoryTimer.Elapsed += SetTime;
            MemoryTimer.Start();

        }

        private void SetTime(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Seconds++;
            });
        }


    }
}
