using Syed.PortfolioPlatform.Application.DTOs.Tags;
using Syed.PortfolioPlatform.Application.Interfaces;
using Syed.PortfolioPlatform.Domain.Entities;

namespace Syed.PortfolioPlatform.Infrastructure.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repository;

        public TagService(ITagRepository repository)
        {
            _repository = repository;
        }

        public async Task<TagDto?> CreateAsync(CreateTagRequest request)
        {
            if (await _repository.IsDuplicateAsync(request.Name, request.Slug))
                return null;

            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug,
                IsDeleted = false
            };

            await _repository.AddAsync(tag);
            await _repository.SaveChangesAsync();

            return MapToDto(tag);
        }

        public async Task<TagDto?> UpdateAsync(Guid id, UpdateTagRequest request)
        {
            var tag = await _repository.GetByIdForUpdateAsync(id);

            if (tag == null)
                return null;

            if (await _repository.IsDuplicateAsync(request.Name, request.Slug, id))
                return null;

            tag.Name = request.Name;
            tag.Slug = request.Slug;

            await _repository.SaveChangesAsync();

            return MapToDto(tag);
        }

        public async Task DeleteAsync(Guid id)
        {
            var tag = await _repository.GetForDeleteAsync(id);

            if (tag == null)
                throw new KeyNotFoundException("Tag not found");

            tag.IsDeleted = true;

            await _repository.SaveChangesAsync();
        }

        public async Task<TagDto?> GetByIdAsync(Guid id)
        {
            var tag = await _repository.GetByIdAsync(id);
            return tag == null ? null : MapToDto(tag);
        }

        public async Task<IEnumerable<TagDto>> GetAllAsync()
        {
            var tags = await _repository.GetAllAsync();
            return tags.Select(MapToDto);
        }

        public async Task<List<TagDto>> GetTagsByCategory(Guid categoryId)
        {
            var tags = await _repository.GetTagsByCategoryAsync(categoryId);

            return tags
                .Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .Distinct()
                .ToList();
        }

        private async Task<bool> IsDuplicateAsync(string name, string slug, Guid? excludeId = null)
        {
            return await _repository.IsDuplicateAsync(name, slug, excludeId);
        }

        private TagDto MapToDto(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                IsDeleted = tag.IsDeleted,
                ArticleCount = tag.ArticleTags?.Count ?? 0
            };
        }
    }
}