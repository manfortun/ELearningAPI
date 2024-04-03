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
public class CoursesController : ErrorHandlingController
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CoursesController(AppDbContext context, IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Policy = nameof(Policy.CoursePolicyOne))]
    public IActionResult GetAll(CourseRetrievalParameters retrievalParams)
    {
        // Include default parameters
        retrievalParams.IncludeParameters.IncludeDefaults<Course>();

        // Create filter layers
        var retrievalManager = new RetrievalManager<Course>(retrievalParams);

        if (retrievalParams.CourseId is int)
        {
            retrievalManager.SetData(_unitOfWork.Courses.Get(c => c.SubjectId == retrievalParams.CourseId, retrievalParams.IncludeParameters.Properties));
        }
        else
        {
            retrievalManager.SetData(_unitOfWork.Courses.Get(include: retrievalParams.IncludeParameters.Properties));
        }

        var courses = retrievalManager.Execute();

        // Build output
        var builder = new RetrieveDtoBuilder<Course, CourseReadDto>();
        var courseRetrieveDto = builder
            .SetMapper(_mapper)
            .SetPaginationParams(retrievalParams.PaginationParameters)
            .SetData(courses)
            .Build();

        return Ok(courseRetrieveDto);
    }

    [HttpPost]
    [Authorize(Policy = nameof(Policy.CoursePolicyFour))]
    public IActionResult Add(CourseCreateDto createDto)
    {
        // Check existence of subject
        if (_unitOfWork.Subjects.Find(createDto.SubjectId) is null)
        {
            return BadRequest("The subject specified does not exist in the database.");
        }

        // Check uniqueness of course title
        if (!TitleUniquenessValidator<Course>.IsUnique(_unitOfWork.Courses, createDto.Title))
        {
            return BadRequest("A course with the same title already exists.");
        }

        var newCourse = new Course
        {
            Title = createDto.Title,
            AuthorId = Convert.ToInt32(TokenService.GetId(HttpContext)),
            SubjectId = createDto.SubjectId,
            Icon = createDto.Icon,
            Description = createDto.Description
        };

        try
        {
            var entityEntry = _unitOfWork.Courses.Insert(newCourse);

            if (entityEntry.State == EntityState.Added)
            {
                _unitOfWork.Save();
            }
            return Ok(_mapper.Map<CourseReadDto>(entityEntry.Entity));
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }

    [HttpGet("{id}")]
    [Authorize(Policy = nameof(Policy.CoursePolicyOne))]
    public IActionResult Get(int id, params string[] join)
    {
        // Always include the owner
        var include = join?.ToHashSet() ?? [];
        include.Add("Author");
        join = [.. include];

        // Check if the id exists
        Course? course = _unitOfWork.Courses.Get(c => c.Id == id, join)?.FirstOrDefault();
        if (course is null)
        {
            return NotFound();
        }

        // Check if the course is published
        if (!course.IsPublished)
        {
            // when not published, the request cannot be completed unless the requestor is the author
            int authorId = Convert.ToInt32(TokenService.GetId(HttpContext));
            if (authorId != course.AuthorId)
            {
                return NotFound();
            }
        }

        var courseReadDto = _mapper.Map<CourseReadDto>(course);

        return Ok(courseReadDto);
    }

    [HttpPatch]
    [Authorize(Policy = nameof(Policy.CoursePolicyFive))]
    [Authorize(Policy = nameof(Policy.OwnerPolicy))]
    public IActionResult Update(int id, CourseUpdateDto courseUpdate)
    {
        // Check if the course exists
        Course? course = _unitOfWork.Courses.Get(c => c.Id == id, "Author")?.FirstOrDefault();
        if (course is null)
        {
            return NotFound();
        }

        // Check if requesting user is the author of the course
        if (course.AuthorId != Convert.ToInt32(TokenService.GetId(HttpContext)))
        {
            return Unauthorized();
        }

        // Check if the title is unique
        if (!TitleUniquenessValidator<Course>.IsUnique(_unitOfWork.Courses, courseUpdate.Title, id))
        {
            return BadRequest("A course with the same title already exists in the database.");
        }

        var entityEditor = new EntityEditor<Course, CourseUpdateDto>();
        course = entityEditor.ApplyEdit(course, courseUpdate);

        try
        {
            var entityEntry = _unitOfWork.Courses.Update(course);

            if (entityEntry.State == EntityState.Modified)
            {
                _unitOfWork.Save();
            }

            return Ok(_mapper.Map<CourseReadDto>(entityEntry.Entity));
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
        // Check that the entity exists
        Course? course = _unitOfWork.Courses.Find(id);
        if (course is null)
        {
            return NotFound();
        }

        try
        {
            var entityEntry = _unitOfWork.Courses.Delete(id);

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
