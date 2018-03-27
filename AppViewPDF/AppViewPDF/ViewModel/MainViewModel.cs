using Acr.UserDialogs;
using AppViewPDF.Interfaces;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppViewPDF.ViewModel
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            OpenPdfCommand = new Command(() => ExecuteOpenPdfCommand());
        }

        public Command OpenPdfCommand { get; }

        private async void ExecuteOpenPdfCommand()
        {

            var urlPdf = "https://download.xamarin.com/developer/xamarin-forms-book/XamarinFormsBook-Ch02-Apr2016.pdf";
            var fileName = Path.GetFileName(urlPdf);

            if (DependencyService.Get<IPdfViewer>().CheckFileExists(fileName))
            {
                DependencyService.Get<IPdfViewer>().ShowFile(fileName);
            }
            else
            {
                using (UserDialogs.Instance.Loading("Carregando pdf...", null, null, true, MaskType.Black))
                {
                    using (var client = new HttpClient())
                    {
                        using (var response = await client.GetAsync(urlPdf))
                        {
                            var result = await response.Content.ReadAsByteArrayAsync();

                            await Task.Run(() => DependencyService.Get<IPdfViewer>().SaveFile(fileName, new MemoryStream(result)));
                        }
                    }

                    DependencyService.Get<IPdfViewer>().ShowFile(fileName);
                }
            }
        }
    }
}
