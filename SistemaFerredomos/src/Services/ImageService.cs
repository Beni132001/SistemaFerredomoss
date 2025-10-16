using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SistemaFerredomos.src.Services
{
    public class ImageService
    {
        private readonly string _imagesDirectory;

        public ImageService()
        {
            // Carpeta en el directorio de la aplicación
            _imagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
            if (!Directory.Exists(_imagesDirectory))
                Directory.CreateDirectory(_imagesDirectory);
        }

        public string SelectAndSaveImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp|Todos los archivos|*.*",
                Title = "Seleccionar imagen"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Generar nombre único para evitar conflictos
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(openFileDialog.FileName)}";
                    var destinationPath = Path.Combine(_imagesDirectory, fileName);

                    // Copiar la imagen a la carpeta de imágenes
                    File.Copy(openFileDialog.FileName, destinationPath, true);
                    return fileName; // Solo guardamos el nombre del archivo en la BD
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al guardar la imagen: {ex.Message}");
                }
            }

            return null;
        }

        public BitmapImage LoadImage(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
                return GetDefaultImage();

            try
            {
                var imagePath = Path.Combine(_imagesDirectory, imageFileName);
                if (!File.Exists(imagePath))
                    return GetDefaultImage();

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imagePath);
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return GetDefaultImage();
            }
        }

        public BitmapImage GetDefaultImage()
        {
            // Imagen por defecto - puedes cambiar esto por una imagen real
            return CreateDefaultBitmapImage();
        }

        private BitmapImage CreateDefaultBitmapImage()
        {
            // Crear una imagen por defecto simple (cuadrado gris)
            var width = 100;
            var height = 100;

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(
                    new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray),
                    new System.Windows.Media.Pen(System.Windows.Media.Brushes.DarkGray, 1),
                    new System.Windows.Rect(0, 0, width, height)
                );

                // Texto "Sin imagen"
                var text = new System.Windows.Media.FormattedText(
                    "Sin imagen",
                    System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight,
                    new System.Windows.Media.Typeface("Arial"),
                    12,
                    System.Windows.Media.Brushes.DarkGray,
                    VisualTreeHelper.GetDpi(visual).PixelsPerDip
                );

                context.DrawText(text, new System.Windows.Point(10, 40));
            }

            var bitmap = new RenderTargetBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            bitmap.Render(visual);

            var bitmapImage = new BitmapImage();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        public void DeleteImage(string imageFileName)
        {
            if (!string.IsNullOrEmpty(imageFileName))
            {
                try
                {
                    var imagePath = Path.Combine(_imagesDirectory, imageFileName);
                    if (File.Exists(imagePath))
                        File.Delete(imagePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar imagen: {ex.Message}");
                }
            }
        }

        public bool ImageExists(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
                return false;

            var imagePath = Path.Combine(_imagesDirectory, imageFileName);
            return File.Exists(imagePath);
        }
    }
} 