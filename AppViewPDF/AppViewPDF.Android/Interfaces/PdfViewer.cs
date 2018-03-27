using Android.Content;
using AppViewPDF.Droid.Interfaces;
using AppViewPDF.Interfaces;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(PdfViewer))]
namespace AppViewPDF.Droid.Interfaces
{
    public class PdfViewer : IPdfViewer
    {
        private string _rootPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path, "ArquivosPDF");

        public void ShowFile(string fileName = "", string mimeType = "")
        {
            var uriFile = Android.Net.Uri.Parse("file://" + Path.Combine(_rootPath, fileName));
            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uriFile, mimeType);
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            Forms.Context.StartActivity(intent);
        }

        public async Task<string> SaveFile(string fileName, Stream pdfStream)
        {
            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);

            var file = Path.Combine(_rootPath, fileName);

            if (File.Exists(file))
                File.Delete(file);

            using (var memoryStream = new MemoryStream())
            {
                await pdfStream.CopyToAsync(memoryStream);
                File.WriteAllBytes(file, memoryStream.ToArray());
            }

            return file;
        }

        public bool CheckFileExists(string fileName)
        {
            return File.Exists(Path.Combine(_rootPath, fileName));
        }
    }
}