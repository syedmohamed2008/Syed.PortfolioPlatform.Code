using Microsoft.Extensions.Configuration;
using Syed.PortfolioPlatform.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syed.PortfolioPlatform.Infrastructure.Storage
{
    public class ImageUrlBuilder : IImageUrlBuilder
    {
        private readonly string _baseUrl;
        private readonly string _container;

        public ImageUrlBuilder(IConfiguration configuration)
        {
            _baseUrl = configuration["BlobStorage:BaseUrl"];
            _container = configuration["BlobStorage:ContainerName"];
        }

        public string Build(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return imagePath!;

            // Already full URL → return as is
            if (imagePath.StartsWith("http"))
                return imagePath;

            return $"{_baseUrl}{_container}/{imagePath}";
        }
    }
}
