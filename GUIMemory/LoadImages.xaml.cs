using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using BusinessMemory;
using Microsoft.Win32;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Forms;  
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Windows.Interop;
using MemoryDataAccess;

namespace GUIMemory
{
    /// <summary>
    /// Interaction logic for LoadImages.xaml
    /// </summary>
    /// 
    public partial class LoadImages : Window
    {
        public int NumberOfCards { get; private set; }
        private const string standardImagePath = "C:\\Users\\ewald\\source\\repos\\Memory\\MemoryGUI\\logo-windesheim.jpeg";

        MemoryCard<BitmapImage>[][] Grid { get; set; }

        private Button ContinueBtn { get; set; }

        private Button NewImagesBtn { get; set; }

        List<BitmapImage> Banners { get; set; }

        public static int UploadCount = 0;
        public LoadImages(int numberOfCards)
        {
            NumberOfCards = numberOfCards;
            Grid = MemoryCard<BitmapImage>.CreateGrid(NumberOfCards / 2);
            InitializeComponent();
            InsertImagesLabel.Content = $"Please upload {NumberOfCards / 2} images";
            Banners = new List<BitmapImage>();
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            if (UploadCount < (NumberOfCards / 2))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Afbeeldingen|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Alle bestanden|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    // Laad de geselecteerde afbeelding in de Image control
                    BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    UploadedImage.Source = bitmap;

                    //check if images are the same
                    if (Banners.Any() && Banners.Any(image => AreBitmapsEqual(bitmap, image)))
                    {
                        MessageBox.Show("Please upload an original image");
                    }
                    else
                    {
                        RandomImagesBtn.Visibility = Visibility.Collapsed;
                        Banners.Add(bitmap);
                        
                        InsertImagesLabel.Content = $"Uploaded {UploadCount + 1}/{NumberOfCards / 2} images";
                        UploadCount++;

                        //enough images have been uploaded
                        if (UploadCount == NumberOfCards / 2)
                        {
                            UploadBannersToDatabase(Banners);
                            MemoryCard<BitmapImage>.StandardBanner = new BitmapImage(new Uri(standardImagePath));
                            MemoryCard<BitmapImage>.SetBanners(Grid, Banners);
                            //ask if they are sure or if they want to continue
                            // Remove the existing button
                            UploadImageBtn.Visibility = Visibility.Collapsed;

                            // Create two new buttons
                            NewImagesBtn = new Button() { Content = "Reset images" };
                            ContinueBtn = new Button() { Content = "Continue" };

                            NewImagesBtn.Click += NewImagesBtn_Click;
                            ContinueBtn.Click += ContinueBtn_Click;
                            // Add the new buttons to the StackPanel
                            myStackPanel.Children.Add(NewImagesBtn);
                            myStackPanel.Children.Add(ContinueBtn);
                        }
                    }
                }

            }

        }

        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            NextMemoryForm();
        }

        private void NextMemoryForm()
        {
            // Open the new window
            MemoryForm memory = new MemoryForm(Grid);
            memory.Show();

            // Close the current window
            this.Close();
        }

        private void NewImagesBtn_Click(object sender, RoutedEventArgs e)
        {
            UploadedImage.Source = null;
            // Reset the MemoryCards array and the UploadCount variable
            Grid = MemoryCard<BitmapImage>.CreateGrid(NumberOfCards / 2);
            Banners.Clear();
            UploadCount = 0;

            // Make the UploadImageBtn button visible again
            UploadImageBtn.Visibility = Visibility.Visible;
            RandomImagesBtn.Visibility = Visibility.Visible;

            // Remove the new buttons from the StackPanel
            myStackPanel.Children.Remove(sender as Button);
            myStackPanel.Children.Remove(ContinueBtn);  // Assuming you have a reference to the ContinueBtn button

            // Reset the label content
            InsertImagesLabel.Content = $"Please upload {NumberOfCards / 2} images";
        }

        private bool AreBitmapsEqual(BitmapImage bitmap1, BitmapImage bitmap2)
        {
            if (bitmap1 != null && bitmap2 != null)
            {


                if (bitmap1.PixelHeight != bitmap2.PixelHeight || bitmap1.PixelWidth != bitmap2.PixelWidth)
                    return false;

                byte[] bitmap1PixelData = new byte[bitmap1.PixelHeight * bitmap1.PixelWidth * 4];
                byte[] bitmap2PixelData = new byte[bitmap2.PixelHeight * bitmap2.PixelWidth * 4];

                bitmap1.CopyPixels(bitmap1PixelData, bitmap1.PixelWidth * 4, 0);
                bitmap2.CopyPixels(bitmap2PixelData, bitmap2.PixelWidth * 4, 0);

                for (int i = 0; i < bitmap1PixelData.Length; i++)
                {
                    if (bitmap1PixelData[i] != bitmap2PixelData[i])
                        return false;
                }
            }

            return true;
        }

        private void RandomImagesBtn_Click(object sender, RoutedEventArgs e)
        {
            ChooseFolderAndGetImages(NumberOfCards/2);
        }

        private void ChooseFolderAndGetImages(int amountOfImages)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string folderPath = dialog.SelectedPath;
                    Banners = GetRandomImagesFromPath(folderPath, amountOfImages);
                    

                    if (Banners.Count < amountOfImages)
                    {
                        MessageBox.Show($"Not enough images in selected folder. Please use another folder with {amountOfImages} images");
                        ChooseFolderAndGetImages(amountOfImages);
                        Banners.Clear();
                    }
                    else
                    {
                        UploadBannersToDatabase(Banners);
                        MemoryCard<BitmapImage>.SetBanners(Grid, Banners);
                        MemoryCard<BitmapImage>.StandardBanner = new BitmapImage(new Uri(standardImagePath));
                        NextMemoryForm();
                    }
                }
            }
        }

        private void UploadBannersToDatabase(List<BitmapImage> banners)
        {
            foreach (BitmapImage image in banners)
            {
                UploadBannerToDatabase(image);
            }
        }

        private void UploadBannerToDatabase(BitmapImage bitmapimage)
        {
            
            // Convert BitmapImage to Bitmap
            byte[] imageData;
            using (MemoryStream ms = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapimage));
                encoder.Save(ms);
                imageData = ms.ToArray();
            }

            if (ImageIsAlreadyStored(imageData))
            {
                return;
            }
            
            MemoryBannerRepository repository = new MemoryBannerRepository();
            int rowsAffected= 0;
            byte[] newRecord = null;
            repository.Create(imageData, out rowsAffected , out newRecord);
            if(rowsAffected == 0)
            {
                int u = 0;
            }

        }

        private bool ImageIsAlreadyStored(byte[] imageData)
        {
            MemoryBannerRepository repository = new MemoryBannerRepository();
            byte[] banner = repository.Read(imageData);
            if(banner == null)
            {
                return false;
            }
            return true;
        }

        private static void ShowImageInMessageBox(Image image)
        {
            // Create a PictureBox control to display the image
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = image;

            // Add PictureBox to a Form
            Form form = new Form();
            form.Text = "Image Popup";
            form.Controls.Add(pictureBox);
            pictureBox.Dock = DockStyle.Fill; // Dock the PictureBox to fill the Form

            // Set Form properties
            form.Size = new System.Drawing.Size(image.Width + 20, image.Height + 40); // Add padding for borders
            form.ShowDialog(); // Show the Form as a modal dialog
        }

        private List<BitmapImage> GetRandomImagesFromPath(string folderPath, int amount)
        {
            // Get all image files in the specified folder
            var files = Directory.EnumerateFiles(folderPath)
                                 .Where(file => file.ToLower().EndsWith("jpg") || file.ToLower().EndsWith("jpeg") ||
                                                file.ToLower().EndsWith("png") || file.ToLower().EndsWith("gif") ||
                                                file.ToLower().EndsWith("bmp"));

            // Create a new Random instance
            var random = new Random();

            // Order the files by random and take the specified amount
            var selectedFiles = files.OrderBy(x => random.Next()).Take(amount).ToList();

            // Convert the selected files to bitmaps
            var selectedImages = selectedFiles.Select(file => new BitmapImage(new Uri(file))).ToList();

            return selectedImages;
        }
    }
}
