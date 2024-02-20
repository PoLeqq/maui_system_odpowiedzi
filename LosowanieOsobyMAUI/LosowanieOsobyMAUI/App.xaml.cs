using Microsoft.Maui.Controls.StyleSheets;

namespace LosowanieOsobyMAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}