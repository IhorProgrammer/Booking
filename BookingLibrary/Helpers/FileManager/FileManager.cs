using Microsoft.AspNetCore.Http;

namespace BookingLibrary.Helpers.FileManager
{
    public class FileManager
    {
        
        public static async Task<string> SaveFile(IFormFile file, string FileSavePath) {


            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), FileSavePath);
            string fullPath = "";
            string fileName = "";
            string extension = Path.GetExtension(file.FileName).ToLower();
            do
            {
                fileName = Guid.NewGuid().ToString() + extension;
                fullPath = Path.Combine(uploadPath, fileName);
            } while (File.Exists(fullPath));

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName; 
        }

        public static bool DeleteFile(string fileName, string FilePath)
        {
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
            File.Delete(Path.Combine(uploadPath, fileName));
            return true;
        }



    }
}
