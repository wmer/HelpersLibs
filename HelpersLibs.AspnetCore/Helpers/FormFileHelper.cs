using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.AspnetCore.Helpers; 
public class FormFileHelper { 
    public static async Task<byte[]> GetByteArray(IFormFile formFile) {
        byte[] file = null;

        if (formFile != null && formFile.Length > 0) {
            var path = Path.Combine(Path.GetTempPath(), formFile.FileName);

            using (var stream = new FileStream(path, FileMode.Create)) {
                await formFile.CopyToAsync(stream);
            }
            if (System.IO.File.Exists(path)) {
                file = System.IO.File.ReadAllBytes(path);
            }
        }

        return file;
    }

    public static async Task<List<byte[]>> GetByteArray(List<IFormFile> formFiles) {
        var files = new List<byte[]>();

        if (formFiles != null && formFiles.Count() > 0) {
            foreach (var formFile in formFiles) {
                files.Add(await GetByteArray(formFile));
            }
        }

        return files;
    }
}
