using BusinessMemory;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Timers;
using System;

namespace GUIMemory
{
    public class MemoryButton : Image
    {
        public MemoryCard<BitmapImage> Card { get; set; }
        private BitmapImage DefaultImage { get; set; }
        public Timer MemoryTimer = new Timer(800);
        public static MemoryForm Form { get; set; }
        public static MemoryButton PreviousButton { get; private set; }
        public static int Clicks = 0;
        public MemoryButton(MemoryCard<BitmapImage> card)
        {

            if(MemoryCard<BitmapImage>.StandardBanner == null)
            {
                throw new Exception("Standard banner must be set");
            }
            else
            {
                DefaultImage = MemoryCard<BitmapImage>.StandardBanner;

            }
            MouseLeftButtonDown += MemoryButton_MouseLeftButtonDown;
            Stretch = Stretch.Fill;
            // Stel in dat de timer het Elapsed event maar één keer activeert.
            MemoryTimer.AutoReset = false;
            // Voeg een event handler toe voor het Elapsed event.
            MemoryTimer.Elapsed += OnTimedEvent;
            Card = card;
            Source = DefaultImage;
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            MemoryTimer.Stop();
            Dispatcher.Invoke(() =>
            {
                Source = DefaultImage;
                PreviousButton.Source = DefaultImage;
                IsEnabled = true; // Schakel de knop weer in
                PreviousButton.IsEnabled = true;
                Clicks = 0;
                Form.Attempts++;
            });
        }

        private void MemoryButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Source == DefaultImage)
            {
                Clicks++;
                if (Clicks < 3)
                {
                    IsEnabled = false;

                    Source = Card.Banner;
                    IsEnabled = false; // Schakel de knop uit
                    if (Clicks == 2)
                    {
                        if (PreviousButton.Card.Id == Card.Id)
                        {
                            Clicks = 0;
                            Form.Attempts++;
                            Form.CardsGuessed += 2;
                        }
                        else
                        {
                            MemoryTimer.Start();
                        }
                    }

                    if (Clicks == 1)
                    {
                        PreviousButton = (MemoryButton)sender;
                    }

                }
            }
        }
    }
}