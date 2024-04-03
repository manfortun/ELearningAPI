using AutoMapper;
using eLearningApi.DataAccess;
using eLearningApi.DTOs;
using eLearningApi.DTOs.Parameters;
using eLearningApi.Services;
using eLearningApi.Services.EntityRetrievalServices;
using eLearningApi.UserRolesAndPermissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using System.Text;
using Content = eLearningApi.Models.Content;

namespace eLearningApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ContentController : ErrorHandlingController
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ContentController(AppDbContext context, IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Policy = nameof(Policy.ContentPolicyOne))]
    public IActionResult GetAll(ContentRetrievalParameters retrievalParams)
    {
        // Always include the owner and module
        retrievalParams ??= new ContentRetrievalParameters();
        retrievalParams.IncludeParameters.IncludeDefaults<Content>();

        IEnumerable<Content> contents = _unitOfWork.Contents.Get(c => c.ModuleId == retrievalParams.ModuleId,
            retrievalParams.IncludeParameters.Properties);

        // check if the content exists
        if (retrievalParams.ContentId is int)
        {
            contents = contents.Where(c => c.Id == retrievalParams.ContentId);
        }

        // Apply keyword filter
        if (retrievalParams.Keyword is not null)
        {
            contents = contents.Where(c =>
            {
                if (c.Type == nameof(Enums.ContentType.Text) || c.Type == nameof(Enums.ContentType.Url))
                {
                    string text = Encoding.UTF8.GetString(c.Material ?? []);
                    return text.Contains(retrievalParams.Keyword, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            });
        }

        // Apply sorting and pagination
        var sortingService = new SortingService<Content>(retrievalParams.SortingParameters);
        var paginationService = new PaginationService<Content>(retrievalParams.PaginationParameters, sortingService);
        paginationService.SetValues(contents);
        paginationService.Execute();
        contents = paginationService.GetValues();

        // Build output
        var builder = new RetrieveDtoBuilder<Content, ContentReadDto>();
        var contentRetrieve = builder
            .SetMapper(_mapper)
            .SetPaginationParams(retrievalParams.PaginationParameters)
            .SetData(contents)
            .Build();

        return Ok(contentRetrieve);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = nameof(Policy.ContentPolicyOne))]
    [Authorize(Policy = nameof(Policy.ContentPolicyTwo))]
    public IActionResult Get(int id)
    {
        // Check if the item exists
        Content? content = _unitOfWork.Contents.Find(id);
        if (content is null)
        {
            return NotFound();
        }

        // Check if the content is published
        if (!content.IsPublished)
        {
            // when not published, the request cannot be completed unless the requestor is the author
            int authorId = Convert.ToInt32(TokenService.GetId(HttpContext));
            if (authorId != content.AuthorId)
            {
                return NotFound();
            }
        }

        var contentRead = _mapper.Map<ContentReadDto>(content);
        return Ok(contentRead);
    }

    [HttpPost]
    [Authorize(Policy = nameof(Policy.ContentPolicyFour))]
    public IActionResult Add(ContentCreateDto contentCreate)
    {
        // Check if the content type is valid
        if (!Enum.GetValues(typeof(ContentType))
            .Cast<ContentType>()
            .Select(c => c.ToString())
            .Contains(contentCreate.Type, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest("The content type is not valid.");
        }

        var newContent = new Content
        {
            Material = contentCreate.Content,
            Type = contentCreate.Type,
            ModuleId = contentCreate.ModuleId
        };

        try
        {
            var entityEntry = _unitOfWork.Contents.Insert(newContent);

            if (entityEntry.State == EntityState.Added)
            {
                _unitOfWork.Save();
                return Ok(_mapper.Map<ContentReadDto>(entityEntry.Entity));
            }
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }

    [HttpPatch]
    [Authorize(Policy = nameof(Policy.ContentPolicyFive))]
    [Authorize(Policy = nameof(Policy.OwnerPolicy))]
    public IActionResult Update(int id, ContentUpdateDto contentUpdate)
    {
        // Check if the content exists
        Content? content = _unitOfWork.Contents.Find(id);
        if (content is null)
        {
            return NotFound();
        }

        var entityEditor = new EntityEditor<Content, ContentUpdateDto>();
        content = entityEditor.ApplyEdit(content, contentUpdate);

        try
        {
            var entityEntry = _unitOfWork.Contents.Update(content);

            if (entityEntry.State == EntityState.Modified)
            {
                _unitOfWork.Save();
            }
            return Ok(_mapper.Map<ContentReadDto>(entityEntry.Entity));
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }

    [HttpDelete]
    [Authorize(Policy = nameof(Policy.CoursePolicySix))]
    [Authorize(Policy = nameof(Policy.CoursePolicySeven))]
    [Authorize(Policy = nameof(Policy.OwnerPolicy))]
    public IActionResult Delete(int id)
    {
        // Check if the content exists
        if (_unitOfWork.Contents.Find(id) is null)
        {
            return NotFound();
        }

        try
        {
            var entityEntry = _unitOfWork.Contents.Delete(id);

            if (entityEntry.State == EntityState.Deleted)
            {
                _unitOfWork.Save();
            }

            return Ok();
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }
}
