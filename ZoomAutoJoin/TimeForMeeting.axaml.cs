using Avalonia;
using Avalonia.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Markup.Xaml;
using static ZoomAutoJoin.MainWindow;
namespace ZoomAutoJoin
{
    public class TimeForMeeting : Window
    {
        public TimeForMeeting() { }
        public TimeForMeeting(Meeting mt)
        {
            this.InitializeComponent(mt);
        }

        private void InitializeComponent(Meeting mt)
        {
            AvaloniaXamlLoader.Load(this);
            var fm = this.FindControl<TextBlock>("meettime");
            fm.Text += $" {mt.info}";
            var es = this.FindControl<Button>("wc");
            var wc = this.FindControl<Button>("nope");
            wc.Click += (_, _) =>
            {
                this.Close();
            };
            es.Click += (_, _) =>
            {
                var url = "https://zoom.us/j/{mt.mid}";
                try
                {
                    Process.Start(url);
                }
                catch
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
            };
        }
    }
}
