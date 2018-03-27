using System.IO;
using System.Threading.Tasks;

namespace AppViewPDF.Interfaces
{
    public interface IPdfViewer
    {
        bool CheckFileExists(string fileName);
        void ShowFile(string filePath = "", string mimeType = "application/pdf");
        Task<string> SaveFile(string fileName, Stream stream);
    }
}
