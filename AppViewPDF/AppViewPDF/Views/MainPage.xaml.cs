using AppViewPDF.ViewModel;
using Xamarin.Forms;

namespace AppViewPDF
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new MainViewModel();
        }
    }
}
