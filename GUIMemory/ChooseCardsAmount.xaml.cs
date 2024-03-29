using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BusinessMemory;

namespace GUIMemory
{
    /// <summary>
    /// Interaction logic for ChooseCardsAmount.xaml
    /// </summary>
    public partial class ChooseCardsAmount : Window
    {
        public ChooseCardsAmount()
        {
            InitializeComponent();
            WelcomeText.Text = $"Welcome to Memory please select an even number of cards you want to play with between {GameRules.MinCards} and {GameRules.MaxCards}";
        }

        private void RandomCardsBtn_Click(object sender, RoutedEventArgs e)
        {

            Random random = new Random();

            // Generate a random integer
            int randomNumber = random.Next(GameRules.MinCards, GameRules.MaxCards);

            // Ensure the generated number is even
            int randomEvenNumber = (randomNumber % 2 == 0) ? randomNumber : randomNumber + 1;

            // Open the new window
            LoadImages newWindow = new LoadImages(randomEvenNumber);
            newWindow.Show();

            // Close the current window

            this.Close();
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string numberFromTextBox = CardsTextBox.Text;
            try
            {
                GameRules.CheckAnwserCardNumber(numberFromTextBox);
                int number = int.Parse(numberFromTextBox.Trim());
                // Open the new window
                LoadImages newWindow = new LoadImages(number);
                newWindow.Show();
                // Close the current window
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
