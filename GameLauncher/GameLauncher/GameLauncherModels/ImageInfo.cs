using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.GameLauncherModels
{
    public class ImageInfo : Models.ModelBase
    {
        private string _imagePath;
        private string _imageDescription;
        private string _imageSubDescription;
        private string _hyperlink;

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                RaisePropertyChanged(nameof(ImagePath));
            }
        }

        public string ImageDescription
        {
            get { return _imageDescription; }
            set
            {
                _imageDescription = value;
                RaisePropertyChanged(nameof(ImageDescription));
            }
        }

        public string ImageSubDescription
        {
            get { return _imageSubDescription; }
            set
            {
                _imageSubDescription = value;
                RaisePropertyChanged(nameof(ImageSubDescription));
            }
        }

        public string Hyperlink
        {
            get { return _hyperlink; }
            set
            {
                _hyperlink = value;
                RaisePropertyChanged(nameof(Hyperlink));
            }
        }
    }
}
