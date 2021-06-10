using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
namespace ZoomAutoJoin
{
    public class MainWindow : Window
    {
        public class Meeting
        {
            public DayToRing dtr { get; set; }
            public TimeSpan ttr { get; set; }
            public string info { get; set; }
            public ulong mid { get; set; }
            public string pwd { get; set; } = "";
            public Meeting() { }
        }
        public class DayToRing
        {
            public bool Monday = false;
            public bool Tuesday = false;
            public bool Wednesday = false;
            public bool Thursday = false;
            public bool Friday = false;
            public bool Saturday = false;
            public bool Sunday = false;
            public int Count { get { return Convert.ToInt32(Monday) + Convert.ToInt32(Tuesday) + Convert.ToInt32(Wednesday) + Convert.ToInt32(Thursday) + Convert.ToInt32(Friday) + Convert.ToInt32(Saturday) + Convert.ToInt32(Saturday); } }
            public DayToRing() { }
        }
        public List<string> meetNamesAndRemove { get { return (File.Exists(path) ? JsonConvert.DeserializeObject<List<Meeting>>(File.ReadAllText(path))?.Select(x => x.info + $"( {x.mid})").ToList() : new List<string>() { "None" }); } }
        public static string path = "meets.json";
        public bool CanSubmit
        {
            get
            {
                if (dys.Count != 0 && text != "") return true;
                else return false;
            }
        }
        public DayToRing dys { get; set; } = new DayToRing();
        public bool Monday { get { return dys.Monday; } set { dys.Monday = value; } }
        public bool Tuesday { get { return dys.Tuesday; } set { dys.Tuesday = value; } }
        public bool Wednesday { get { return dys.Wednesday; } set { dys.Wednesday = value; } }
        public bool Thursday { get { return dys.Thursday; } set { dys.Thursday = value; } }
        public bool Friday { get { return dys.Friday; } set { dys.Friday = value; } }
        public bool Saturday { get { return dys.Saturday; } set { dys.Saturday = value; } }
        public bool Sunday { get { return dys.Sunday; } set { dys.Sunday = value; } }
        public string stext
        {
            get { return text; }
            set
            {
                var x = this.FindControl<TextBox>("minfo");
                if (value == "") x.Background = new SolidColorBrush(Color.Parse("#fd7d00"));
                else { text = value; x.Background = new SolidColorBrush(Color.Parse("#400063bb")); }
            }
        }
        /// <summary>
        /// The Time it gets (and sets) for the Zoom Meeting
        /// </summary>
        public TimeSpan timeSpan { get; set; } = new TimeSpan(12, 00, 0);
        public string text { get; set; } = "New Meeting";
        public string _MeetID
        {
            get { return MeetID.ToString(); }
            set
            {
                var x = this.FindControl<TextBox>("meetid");
                if (!ulong.TryParse(value, out ulong mid))
                {
                    x.Foreground = new SolidColorBrush(Color.Parse("#fd7d00"));
                    ToolTip.SetTip(x, "Enter a number!!!");
                    return;
                }
                else
                {
                    ToolTip.SetTip(x, null);
                    ToolTip.SetIsOpen(x, false);
                    x.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    MeetID = mid;
                }
            }
        }
        public ulong MeetID { get; set; } = 67834597763570865;
        public string pass { get; set; } = "";
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Button submissionButton = this.FindControl<Button>("sub");
            Button bg = this.FindControl<Button>("bg");
            ListBox hah = this.FindControl<ListBox>("hah");
            Button remove = this.FindControl<Button>("rem");
            ComboBox cbx = this.FindControl<ComboBox>("cbx");
            cbx.SelectionChanged += (_, scer) =>
            {
                if (cbx.SelectedItem == null) remove.IsEnabled = false;
                else remove.IsEnabled = true;
            };
            remove.Click += (sc, am) =>
            {
                if (cbx.SelectedItem == null) return;
                var str = cbx.SelectedItem.ToString();
                var ral = JsonConvert.DeserializeObject<List<Meeting>>(File.ReadAllText(path));
                ral.RemoveAll(x => x.info + $"( {x.mid})" == str);
                File.WriteAllText(path, JsonConvert.SerializeObject(ral));
                cbx.Items = ral.Select(x => x.info + $"( {x.mid})");
            };
            hah.SelectionMode = SelectionMode.Multiple;
            TabControl tc = this.FindControl<TabControl>("tc");
            tc.SelectionChanged += (_, _) =>
            {
                try
                {
                    cbx.Items = meetNamesAndRemove;
                } catch { }
            };
            hah.SelectionChanged += (x, y) =>
            {
                if (CanSubmit)
                {
                    submissionButton.IsEnabled = true;
                }
                else
                {
                    submissionButton.IsEnabled = false;
                }
            };
            submissionButton.Click += HandleSubmissionClick;
            bg.Click += (_, _) =>
            {
                this.Hide();
            };           
        }

        private void HandleSubmissionClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mt = new Meeting
            {
                dtr = dys,
                ttr = timeSpan,
                info = text,
                mid = MeetID,
                pwd = pass
            };
            if (File.Exists(path))
            {
                var txt = File.ReadAllText(path);
                List<Meeting> lm = JsonConvert.DeserializeObject<List<Meeting>>(txt);
                if (lm == null) { lm = new List<Meeting>(); }
                lm.Add(mt);
                var rettxt = JsonConvert.SerializeObject(lm);
                File.WriteAllText(path, rettxt);
            }
            else
            {
                List<Meeting> metList = new List<Meeting>() { mt };
                var rettxt = JsonConvert.SerializeObject(metList);
                File.WriteAllText(path, rettxt);
            }
            var pes = new PopupYes();
            pes.Show();
            ComboBox cbx = this.FindControl<ComboBox>("cbx");
            cbx.Items = meetNamesAndRemove;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}