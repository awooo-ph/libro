using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Libro.Converters {
    class PathToBitmapConverter : ConverterBase {
      

        public PathToBitmapConverter()
        {
      
        }
        

        private static Dictionary<string, BitmapImage> Cache = new Dictionary<string, BitmapImage>();

        protected override object Convert(object value, Type targetType, object parameter)
        {

            // var defUri = _imageType ==ImageType.Background ? new Uri($"pack://application:,,,/Resources/background.jpg"): new Uri("pack://application:,,,/Resources/user.png");
            BitmapImage bmp = null;
            if (value==null || !File.Exists((string) value))
            {
             
            }
            else
            {
                try
                {
                    using (var stream = File.OpenRead((string)value))
                    {
                        bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.StreamSource = stream;
                        bmp.UriSource = File.Exists((string)value) ? new Uri($"file://{(string)Path.GetFullPath((string)value)}") : null;
                        bmp.EndInit();
                        bmp.Freeze();
                    }
                }
                catch 
                {
                    if (Cache.ContainsKey((string) value))
                    {
                        bmp = Cache[(string) value];
                    }
                    else
                    {
                        return null;
                    }
                }
                
            }

            var key = value?.ToString() ?? "[EMPTY]";
            if (Cache.ContainsKey(key))
                Cache[key] = bmp;
            else
                Cache.Add(key,bmp);

            return bmp;
            
        }

        
    }
}
