using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLibrary.Services.FileManager
{
    public class FileManager
    {
        
        public static async Task<string> SaveFile(IFormFile file, string FileSavePath) {


            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), FileSavePath);
            string fullPath = "";
            string fileName = "";
            string extension = Path.GetExtension(file.FileName);
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



    }
}
