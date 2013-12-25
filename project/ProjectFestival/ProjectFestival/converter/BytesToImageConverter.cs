using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;

namespace ProjectFestival.converter
{
    public class BytesToImageConverter : IValueConverter
    {
        public static BitmapImage imageSource = new BitmapImage();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] bytes = value as byte[];

            if (value != null && value is byte[] && bytes.Length != 0)
            { 
                imageSource = BitmapImageFromBytes(bytes);
                return imageSource;
            }
            else
            {
                return new BitmapImage(new Uri(@"C:\Users\alisio\Desktop\howest\2NMCT\Business Applications\oefeningen\Atraction\Atraction\images\noimage.png"));
            }
        }

        private BitmapImage BitmapImageFromBytes(byte[] bytes)
        {
            BitmapImage image = null;
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(bytes);
                stream.Seek(0, SeekOrigin.Begin);
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                image = new BitmapImage();
                image.BeginInit();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.StreamSource.Seek(0, SeekOrigin.Begin);
                image.EndInit();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
