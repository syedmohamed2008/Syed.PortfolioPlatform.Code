using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syed.PortfolioPlatform.Application.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
