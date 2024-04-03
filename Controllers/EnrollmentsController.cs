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
public class EnrollmentsController : ErrorHandlingController
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EnrollmentsController(AppDbContext context, IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Policy = nameof(Policy.EnrollmentPolicyOne))]
    public IActionResult GetAll(EnrollmentRetrievalParameters retrievalParams)
    {
        // Always include the owner, enrollment modules, and course
        var defaults = IncludeParameters.GetDefaults<EnrollmentModule>().ToArray();
        var enrollmentModules = _unitOfWork.EnrollmentModules.Get(include: defaults);

        // Apply filters
        if (retrievalParams.StudentId is int)
        {
            enrollmentModules = enrollmentModules.Where(e => e.Enrollment?.UserId == retrievalParams.StudentId);
        }

        if (retrievalParams.IsCompleted is bool)
        {
            enrollmentModules = enrollmentModules.Where(e => e.IsCompleted == retrievalParams.IsCompleted);
        }

        if (retrievalParams.CourseIds is not null && retrievalParams.CourseIds.Any())
        {
            enrollmentModules = enrollmentModules.Where(e => retrievalParams.CourseIds.Contains(e.Enrollment.CourseId));
        }

        // Obtain enrollments from enrollment IDs
        // Always include the owner and course
        retrievalParams.IncludeParameters.IncludeDefaults<Enrollment>();

        var enrollmentIds = enrollmentModules.Select(e => e.Enrollment.Id);
        var enrollments = _unitOfWork.Enrollments.Get(e => enrollmentIds.Contains(e.Id), retrievalParams.IncludeParameters.Properties);

        // Apply sorting and pagination
        var sortingService = new SortingService<Enrollment>(retrievalParams.SortingParameters);
        var pagingService = new PaginationService<Enrollment>(retrievalParams.PaginationParameters, sortingService);

        pagingService.SetValues(enrollments);
        pagingService.Execute();
        enrollments = pagingService.GetValues();

        // Build output
        var builder = new RetrieveDtoBuilder<Enrollment, EnrollmentReadDto>();
        var retrieveDto = builder
            .SetMapper(_mapper)
            .SetPaginationParams(retrievalParams.PaginationParameters)
            .SetData(enrollments)
            .Build();

        return Ok(retrieveDto);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = nameof(Policy.EnrollmentPolicyTwo))]
    public IActionResult Get(int id, params string[] join)
    {
        // Always include the owner and course
        var include = join?.ToHashSet() ?? [];
        include.Add("User");
        include.Add("Course");
        join = [.. include];

        // Check if the enrollment exists
        Enrollment? enrollment = _unitOfWork.Enrollments.Get(e => e.Id == id, join)?.FirstOrDefault();
        if (enrollment is null)
        {
            return NotFound();
        }

        var enrollmentDto = _mapper.Map<EnrollmentReadDto>(enrollment);
        return Ok(enrollmentDto);
    }

    [HttpPost]
    [Authorize(Policy = nameof(Policy.EnrollmentPolicyOne))]
    public IActionResult Add(int id)
    {
        // Check that the course exists
        Course? course = _unitOfWork.Courses.Get(c => c.Id == id, "Modules")?.FirstOrDefault();
        if (course is null)
        {
            return NotFound();
        }

        // Create new enrollment
        var enrollment = new Enrollment
        {
            CourseId = id,
            UserId = Convert.ToInt32(TokenService.GetId(HttpContext)),
        };

        try
        {
            var entityEntry = _unitOfWork.Enrollments.Insert(enrollment);

            if (entityEntry.State == EntityState.Added)
            {
                _unitOfWork.Save();
                enrollment = entityEntry.Entity;
            }
        }
        catch
        {
            return InternalError();
        }

        List<Module> unsuccessfulModules = [];
        // Create enrollment for each module
        foreach (var module in course.Modules)
        {
            var enrollmentModule = new EnrollmentModule
            {
                EnrollmentId = enrollment.Id,
                IsCompleted = false,
                ModuleId = module.Id,
            };

            try
            {
                var entityEntry = _unitOfWork.EnrollmentModules.Insert(enrollmentModule);
                if (entityEntry.State != EntityState.Added)
                {
                    unsuccessfulModules.Add(module);
                }
            }
            catch
            {
                unsuccessfulModules.Add(module);
            }
        }

        if (unsuccessfulModules.Any())
        {
            return InternalError("The student could not be registered to some modules");
        }
        else
        {
            // Save the created enrollment modules
            _unitOfWork.Save();
            return Ok();
        }
    }

    [HttpDelete]
    [Authorize(Policy = nameof(Policy.EnrollmentPolicyFive))]
    public IActionResult Delete(int id)
    {
        // Check if the enrollment exists
        if (_unitOfWork.Enrollments.Find(id) is null)
        {
            return NotFound();
        }

        try
        {
            var entityEntry = _unitOfWork.Enrollments.Delete(id);

            if (entityEntry.State == EntityState.Deleted)
            {
                _unitOfWork.Save();
                return Ok();
            }
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }
}
