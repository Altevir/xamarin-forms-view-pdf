using Android.Content;
using Android.Support.V4.Content;
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
            Android.Net.Uri uriFile = null;
            var intent = new Intent(Intent.ActionView);

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
            {
                //Versões de API >= 24 não é mais possível expor um arquivo com file:// através de um Intent fora do seu domínio de pacote
                //exception gerada: FileUriExposedException
                //Exemplo de como é retornado agora: "content://com.companyname.AppViewPDF.fileprovider/external_files/Download/ArquivosPDF/XamarinFormsBook-Ch02-Apr2016.pdf"
                //--------------------------------------------------------------------
                //Abaixo rotina que corrige e dá acesso a gravação/leitura de arquivos
                //Deve ser usado agora o FileProvider, o qual permite compartilhar arquivos com segurança
                //OBS: O FileProvider tem que ser adicionado no arquivo de AndroidManifest.xml dentro de application
                //Verificar o arquivo Resources > xml > file_paths.xml > external_files
                //---------------------------------------------------------------------
                intent.SetFlags(ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission);
                Java.IO.File file = new Java.IO.File(Path.Combine(_rootPath, fileName));
                uriFile = FileProvider.GetUriForFile(Forms.Context.ApplicationContext, Forms.Context.ApplicationContext.PackageName + ".fileprovider", file);
            }
            else
            {
                uriFile = Android.Net.Uri.Parse("file://" + Path.Combine(_rootPath, fileName));
            }

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