using GameLauncher.GameLauncherModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.ViewModels
{
    public class MainWindowViewModel : Models.ModelBase
    {
        public MainWindowViewModel()
        {
            ImageData = new ImageInfo()
            {
                ImagePath = "C:\\ParkerrrWebsite\\Images\\GameImage1.png",
                ImageDescription = "Learn about the game",
                ImageSubDescription = "Click here to learn more about the game.\nWe're always adding new things.",
                Hyperlink = "http://parkerrr.com/home/about"
            };
        }

        private ImageInfo _imageData;

        public ImageInfo ImageData
        {
            get { return _imageData; }
            set
            {
                _imageData = value;
                RaisePropertyChanged(nameof(ImageData));
            }
        }
    }
}
