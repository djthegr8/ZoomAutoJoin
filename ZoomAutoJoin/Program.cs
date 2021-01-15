using Avalonia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Threading;

namespace ZoomAutoJoin
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static bool wheee { get; set; } = true;
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            // Hiding the window, as it makes the app feel better.
            ShowWindow(handle, 0);
            // Required for MacOS
            Directory.SetCurrentDirectory(Path.GetDirectoryName(AppContext.BaseDirectory));
            var ss = AppBuilder.Configure<App>().UsePlatformDetect();
            var tmr = new Timer(new TimerCallback(x =>
            {
                if (!wheee) return;
                var currTime = DateTime.Now;
                if (!File.Exists(MainWindow.path)) File.Create("meets.json"); 
                var text = File.ReadAllText(MainWindow.path);
                if (text == "") return;
                List<Meeting> LoMs = JsonConvert.DeserializeObject<List<Meeting>>(text);
                if (LoMs.Any(k =>
                {
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
                    var isSameDay = currTime.DayOfWeek.ToString() switch
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
                    {
                        "Monday" => k.dtr.Monday == true,
                        "Tuesday" => k.dtr.Tuesday == true,
                        "Wednesday" => k.dtr.Wednesday == true,
                        "Thursday" => k.dtr.Thursday == true,
                        "Friday" => k.dtr.Friday == true,
                        "Saturday" => k.dtr.Saturday == true,
                        "Sunday" => k.dtr.Sunday == true
                    };
                    if (!isSameDay) return false;
                    var isSameTime = currTime.TimeOfDay.Hours == k.ttr.Hours && currTime.TimeOfDay.Minutes == k.ttr.Minutes;
                    if (!isSameTime) return false;
                    return true;
                })) {
                    Console.WriteLine("ITS TIME");
                    var idek = LoMs.First(k =>
                    {
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
                        var isSameDay = currTime.DayOfWeek.ToString() switch
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
                        {
                            "Monday" => k.dtr.Monday == true,
                            "Tuesday" => k.dtr.Tuesday == true,
                            "Wednesday" => k.dtr.Wednesday == true,
                            "Thursday" => k.dtr.Thursday == true,
                            "Friday" => k.dtr.Friday == true,
                            "Saturday" => k.dtr.Saturday == true,
                            "Sunday" => k.dtr.Sunday == true
                        };
                        if (!isSameDay) return false;
                        var isSameTime = currTime.TimeOfDay.Hours == k.ttr.Hours && currTime.TimeOfDay.Minutes == k.ttr.Minutes;
                        if (!isSameTime) return false;
                        return true;
                    });
                    var url = $"zoommtg://zoom.us/join?confno={idek.mid}";
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
                    wheee = false;
                    Thread.Sleep(60000);
                    wheee = true;
                }
            }), null, 0, 2000);
            ss.StartWithClassicDesktopLifetime(args, Avalonia.Controls.ShutdownMode.OnMainWindowClose);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();

    }
}
