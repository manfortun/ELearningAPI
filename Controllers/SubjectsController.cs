using AutoMapper;
using eLearningApi.DataAccess;
using eLearningApi.DTOs;
using eLearningApi.DTOs.Parameters;
using eLearningApi.Models;
using eLearningApi.Services;
using eLearningApi.Services.EntityRetrievalServices;
using eLearningApi.UserRolesAndPermissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLearningApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectsController : ErrorHandlingController
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubjectsController(AppDbContext context, IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieve the list of subjects with filtering, searching, sorting, and pagination.
    /// </summary>
    /// <param name="sort">Column to sort. Used in conjunction with sortDirection.</param>
    /// <param name="sortDirection">Describes the direction of the sort. Possible values are "ASC" and "DESC".</param>
    /// <param name="keyword">Keyword to search.</param>
    /// <param name="published">To filter published or draft subjects.</param>
    /// <param name="courses">To filter subjects that have courses.</param>
    /// <param name="limit">Number of results to return. Default value is 25.</param>
    /// <param name="page">Requested page number. Default value is 1.</param>
    /// <param name="join">Entities to be included with.</param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = nameof(Policy.SubjectPolicyOne))]
    public IActionResult GetAll(RetrievalParameters? retrievalParams)
    {
        // Include default parameters
        retrievalParams ??= new RetrievalParameters();
        retrievalParams.IncludeParameters.IncludeDefaults<Subject>();

        // Create filter layers
        var retrievalManager = new RetrievalManager<Subject>(retrievalParams);
        retrievalManager.SetData(_unitOfWork.Subjects.Get(include: retrievalParams.IncludeParameters.Properties));
        var subjects = retrievalManager.Execute();

        // build output
        var builder = new RetrieveDtoBuilder<Subject, SubjectReadDto>();
        var subjectRetrieve = builder
            .SetMapper(_mapper)
            .SetPaginationParams(retrievalParams.PaginationParameters)
            .SetData(subjects)
            .Build();

        return Ok(subjectRetrieve);
    }

    [HttpPost]
    [Authorize(Policy = nameof(Policy.SubjectPolicyFour))]
    public IActionResult Add(string title)
    {
        // Ensure that the title is unique
        if (!TitleUniquenessValidator<Subject>.IsUnique(_unitOfWork.Subjects, title))
        {
            return BadRequest("A subject with the same title already exists in the database.");
        }

        var newSubject = new Subject
        {
            Title = title,
            AuthorId = Convert.ToInt32(TokenService.GetId(HttpContext))
        };

        try
        {
            var entityEntry = _unitOfWork.Subjects.Insert(newSubject);

            if (entityEntry.State == EntityState.Added)
            {
                _unitOfWork.Save();
            }

            var subject = _unitOfWork.Subjects.Get(s => s.Id == entityEntry.Entity.Id, "Author");
            return Ok(_mapper.Map<SubjectReadDto>(subject.First()));
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }

    [HttpGet("{id}")]
    [Authorize(Policy = nameof(Policy.SubjectPolicyOne))]
    public IActionResult Get(int id, [FromQuery] string[] join)
    {
        // Always include the owner
        var include = join?.ToHashSet() ?? [];
        include.Add("Author");
        join = [.. include];

        Subject? subject = _unitOfWork.Subjects.Get(s => s.Id == id, include: join)?.FirstOrDefault();
        if (subject is null)
        {
            return NotFound();
        }

        // Check if the subject is published
        if (!subject.IsPublished)
        {
            // when not published, the request cannot be completed unless the requestor is the author
            int authorId = Convert.ToInt32(TokenService.GetId(HttpContext));
            if (authorId != subject.AuthorId)
            {
                return NotFound();
            }
        }

        var subjectRead = _mapper.Map<SubjectReadDto>(subject);

        return Ok(subjectRead);
    }

    [HttpPatch]
    [Authorize(Policy = nameof(Policy.SubjectPolicyFive))]
    [Authorize(Policy = nameof(Policy.OwnerPolicy))]
    public IActionResult Update(int id, SubjectUpdateDto subjectUpdate)
    {
        // Check if the subject exists
        Subject? subject = _unitOfWork.Subjects.Get(s => s.Id == id, "Author")?.FirstOrDefault();
        if (subject is null)
        {
            return NotFound();
        }

        // Check if requesting user is the author of the subject
        if (subject.AuthorId != Convert.ToInt32(TokenService.GetId(HttpContext)))
        {
            return Unauthorized();
        }

        // Check if the title is unique
        if (!TitleUniquenessValidator<Subject>.IsUnique(_unitOfWork.Subjects, subjectUpdate.Title, id))
        {
            return BadRequest("The subject title is not unique.");
        }

        var entityEditor = new EntityEditor<Subject, SubjectUpdateDto>();
        subject = entityEditor.ApplyEdit(subject, subjectUpdate);

        try
        {
            var entityEntry = _unitOfWork.Subjects.Update(subject);

            if (entityEntry.State == EntityState.Modified)
            {
                _unitOfWork.Save();
            }
            return Ok(_mapper.Map<SubjectReadDto>(entityEntry.Entity));
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }

    [HttpDelete]
    [Authorize(Policy = nameof(Policy.SubjectPolicySix))]
    [Authorize(Policy = nameof(Policy.SubjectPolicySeven))]
    [Authorize(Policy = nameof(Policy.OwnerPolicy))]
    public IActionResult Delete(int id)
    {
        // Check if the record exists
        Subject? subject = _unitOfWork.Subjects.Find(id);
        if (subject is null)
        {
            return NotFound();
        }

        try
        {
            var entityEntry = _unitOfWork.Subjects.Delete(subject);

            if (entityEntry.State == EntityState.Deleted)
            {
                _unitOfWork.Save();
            }
            return Ok();
        }
        catch
        {
            return InternalError();
        }
    }

}
