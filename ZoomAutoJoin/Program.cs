using Avalonia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
namespace ZoomAutoJoin
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            var tmr = new Timer(new TimerCallback(async x =>
            {
                var currTime = DateTime.Now;
                var text = await File.ReadAllTextAsync(MainWindow.path);
                List<Meeting> LoMs = JsonConvert.DeserializeObject<List<Meeting>>(text);
                if (LoMs.Any(k =>
                {
                    var isSameDay = currTime.DayOfWeek.ToString() switch
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

                }
            }), null, 0, 1000);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();

    }
}
