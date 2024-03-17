using GameLauncher.GameLauncherModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameLauncher.Controls
{
    /// <summary>
    /// Interaction logic for ImageWithInfo.xaml
    /// </summary>
    public partial class ImageWithInfo : UserControl
    {
        public ImageWithInfo()
        {
            InitializeComponent();
        }

        private void ImageWithInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var imageData = this.DataContext as ImageInfo;
            if (imageData == null)
            {
                return;
            }
            System.Diagnostics.Process.Start(imageData.Hyperlink);
        }
    }
}
