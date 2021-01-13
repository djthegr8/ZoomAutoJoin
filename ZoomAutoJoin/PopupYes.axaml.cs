using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZoomAutoJoin
{
    public class PopupYes : Window
    {
        public PopupYes()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var wc = this.FindControl<Button>("wc");
            wc.Click += (_, _) =>
            {
                this.Close();
            };
        }
    }
}
