using MemoryDataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Drawing;
using Image = System.Drawing.Image; // Add this line at the top of your code file

namespace GUIMemory
{
    /// <summary>
    /// Interaction logic for ViewImages.xaml
    /// </summary>
    public partial class ViewImages : Window
    {
        public ObservableCollection<BitmapImage> Images { get; set; }
        public ViewImages()
        {
            InitializeComponent();
            DataContext = this;
            Images = new ObservableCollection<BitmapImage>();
            // Assuming you have a method to load images from the database and store them in a collection
            LoadImagesFromDatabase();
            SetImages();
        }

        private void SetImages()
        {
            foreach (BitmapImage image in Images)
            {
                imagesList.Items.Add(image);
            }
        }

        private void LoadImagesFromDatabase()
        {
            // Your logic to load images from the database and return a collection of BitmapImage
            MemoryBannerRepository repository = new MemoryBannerRepository();
            List<byte[]> banners = repository.ReadAll();
            foreach (byte[] banner in banners)
            {
                BitmapImage image = ConvertByteArrayToBitmapImage(banner);
                Images.Add(image);
            }   
        }
            
        public BitmapImage ConvertByteArrayToBitmapImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; 
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
