namespace Syed.PortfolioPlatform.Application.DTOs.Tags
{
    public class CreateTagRequest
    {
        public string Name { get; set; } = null!;
        public string? Slug { get; set; } = null!;
    }
}

