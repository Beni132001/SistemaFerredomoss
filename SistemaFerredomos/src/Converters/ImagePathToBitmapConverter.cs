using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using SistemaFerredomos.src.Services;

namespace SistemaFerredomos.src.Converters
{
    public class ImagePathToBitmapConverter : IValueConverter
    {
        private static readonly ImageService _imageService = new ImageService();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imagePath = value as string;
            return _imageService.LoadImage(imagePath);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}