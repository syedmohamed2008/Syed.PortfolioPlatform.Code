using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Syed.PortfolioPlatform.Application.DTOs.Articles;
using System.Text.Json.Serialization;

public class UpdateArticleRequest : CreateArticleRequest
{
    public Guid Id { get; set; }



}
