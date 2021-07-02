using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace StockBackendMongo.Repositories
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration configuration;

        public UploadFileService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.configuration = configuration;
        }

        public bool IsUpload(List<IFormFile> formFiles)
        {
            bool result = formFiles != null && formFiles.Sum(f => f.Length) > 0;
            return result;
        }

        public Task<List<string>> UploadImage(List<IFormFile> formFiles)
        {
            throw new NotImplementedException();
        }





        public async Task<List<string>> UploadImages(List<IFormFile> formFiles)
        {
            List<string> listFileName = new List<string>();
            string uploadPath = $"{webHostEnvironment.WebRootPath}/images/";


            foreach (var formFile in formFiles)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                string fullPath = uploadPath + fileName;
                using (var stream = File.Create(fullPath))
                {
                    await formFile.CopyToAsync(stream);
                }
                listFileName.Add(fileName);
            }

            return listFileName;
        }

        public Task RemoveImage(string image)
        {
            string uploadPath = $"{webHostEnvironment.WebRootPath}/images/";

            string fullPath = uploadPath + image;
            File.Delete(fullPath);
            return Task.CompletedTask;
        }

        public string Validation(List<IFormFile> formFiles)
        {
            foreach (var formFile in formFiles)
            {
                if (!ValidationExtension(formFile.FileName)) return "Invalid file extension";

                if (!ValidationSize(formFile.Length)) return "The file is too large";

            }
            return null;
        }

        public bool ValidationExtension(string fileName)
        {
            string[] permittedExtentons = { ".jpg", ".png", ".jpeg" };
            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtentons.Contains(ext))
            {
                return false;
            }
            return true;
        }

        public bool ValidationSize(long fileSize)
        {
            bool result = configuration.GetValue<long>("FileSizeLimit") > fileSize;
            return result;
        }

    }
}