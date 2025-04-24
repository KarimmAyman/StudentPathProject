using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.FileServices
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile file, string folderName);
    }
}
