using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Libro.Properties;

namespace Libro
{
    static class Extensions
    {
        public static readonly DependencyProperty PlayClickProperty = DependencyProperty.RegisterAttached(
            "PlayClick", typeof(bool), typeof(Extensions), new PropertyMetadata(default(bool), OnPlayClickChanged));

        private static void OnPlayClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement f)) return;
            f.MouseDown += (sender, args) =>
            {
                Resources.click_02.Play();
            };
        }

        public static void SetPlayClick(DependencyObject element, bool value)
        {
            element.SetValue(PlayClickProperty, value);
        }

        public static bool GetPlayClick(DependencyObject element)
        {
            return (bool) element.GetValue(PlayClickProperty);
        }

        public static async void Play(this UnmanagedMemoryStream res)
        {
            using (var player = new SoundPlayer(res))
            {
                player.Play();
            }
        }

        public static async void Log(this Exception exception, string source = null, bool includeStackTrace = false)
        {
            if (exception == null) return;
            if (!string.IsNullOrEmpty(source))
                source = $" [{source}]";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    File.AppendAllText(Path.Combine(".","Error.log"), $@"{DateTime.Now:d} {source}: {exception?.Message}{Environment.NewLine}");
                    if (includeStackTrace)
                    {
                        File.AppendAllText(Path.Combine(".","Error.log"),$@"{exception.StackTrace} {Environment.NewLine}");
                    }
                }
                catch (Exception e)
                {
                    //
                }
            });
        }
        
        public static string ToTitleCase(this string text, string separator = " ")
        {
            if (string.IsNullOrEmpty(text)) return "";
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            string lowerText = textInfo.ToLower(text);
            string[] words = lowerText.Split(new[] {separator}, StringSplitOptions.None);

            return String.Join(separator, words.Select(v => textInfo.ToTitleCase(v)));
        }
    }
}
