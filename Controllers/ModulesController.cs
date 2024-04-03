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
public class ModulesController : ErrorHandlingController
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ModulesController(AppDbContext context, IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Policy = nameof(Policy.ModulePolicyOne))]
    public IActionResult GetAll(ModuleRetrievalParameters retrievalParams)
    {
        // Include default properties
        retrievalParams.IncludeParameters.IncludeDefaults<Module>();

        // Create filter layer
        var retrievalManager = new RetrievalManager<Module>(retrievalParams);

        IEnumerable<Module> modules;
        if (retrievalParams.ModuleId is int)
        {
            modules = _unitOfWork.Modules.Get(m => m.CourseId == retrievalParams.ModuleId, retrievalParams.IncludeParameters.Properties);
        }
        else
        {
            modules = _unitOfWork.Modules.Get(include: retrievalParams.IncludeParameters.Properties);
        }

        if (retrievalParams.Duration is int)
        {
            modules = modules.Where(m => m.Duration >= retrievalParams.Duration);
        }

        retrievalManager.SetData(modules);
        var filteredModules = retrievalManager.Execute();

        // Build output
        var builder = new RetrieveDtoBuilder<Module, ModuleReadDto>();
        var moduleRetrieve = builder
            .SetMapper(_mapper)
            .SetPaginationParams(retrievalParams.PaginationParameters)
            .SetData(filteredModules)
            .Build();

        return Ok(moduleRetrieve);
    }

    [HttpPost]
    [Authorize(Policy = nameof(Policy.ModulePolicyFour))]
    public IActionResult Add(ModuleCreateDto moduleCreate)
    {
        // Check if the course exists
        if (_unitOfWork.Courses.Find(moduleCreate.CourseId) is null)
        {
            return BadRequest("The course specified does not exist in the database.");
        }

        var newModule = new Module
        {
            Title = moduleCreate.Title,
            AuthorId = Convert.ToInt32(TokenService.GetId(HttpContext)),
            CourseId = moduleCreate.CourseId,
            Duration = moduleCreate.Duration,
        };

        try
        {
            var entityEntry = _unitOfWork.Modules.Insert(newModule);

            if (entityEntry.State == EntityState.Added)
            {
                _unitOfWork.Save();
                return Ok(_mapper.Map<ModuleReadDto>(entityEntry.Entity));
            }
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }


    [HttpGet("{id}")]
    [Authorize(Policy = nameof(Policy.ModulePolicyOne))]
    public IActionResult Get(int id, params string[] join)
    {
        // Always include the contents
        var include = join?.ToHashSet() ?? [];
        include.Add("Contents");
        join = [.. include];

        // Check if the module exists
        Module? module = _unitOfWork.Modules.Get(m => m.Id == id, join)?.FirstOrDefault();
        if (module is null)
        {
            return NotFound();
        }

        // Check if the module is published
        if (!module.IsPublished)
        {
            // when not published, the request cannot be completed unless the requestor is the author
            int authorId = Convert.ToInt32(TokenService.GetId(HttpContext));
            if (authorId != module.AuthorId)
            {
                return NotFound();
            }
        }

        var moduleDto = _mapper.Map<ModuleReadDto>(module);

        return Ok(moduleDto);
    }

    [HttpPatch]
    [Authorize(Policy = nameof(Policy.ModulePolicyFive))]
    [Authorize(Policy = nameof(Policy.OwnerPolicy))]
    public IActionResult Update(int id, ModuleUpdateDto moduleUpdate)
    {
        // Check if the module exists in the database
        Module? module = _unitOfWork.Modules.Find(id);
        if (module is null)
        {
            return NotFound();
        }

        var entityEditor = new EntityEditor<Module, ModuleUpdateDto>();
        module = entityEditor.ApplyEdit(module, moduleUpdate);

        try
        {
            var entityEntry = _unitOfWork.Modules.Update(module);

            if (entityEntry.State == EntityState.Modified)
            {
                _unitOfWork.Save();
            }

            return Ok(_mapper.Map<ModuleReadDto>(entityEntry.Entity));
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }

    [HttpDelete]
    [Authorize(Policy = nameof(Policy.ModulePolicySix))]
    [Authorize(Policy = nameof(Policy.ModulePolicySeven))]
    [Authorize(Policy = nameof(Policy.OwnerPolicy))]
    public IActionResult Delete(int id)
    {
        // Check if module exists
        if (_unitOfWork.Modules.Find(id) is null)
        {
            return NotFound();
        }

        try
        {
            var entityEntry = _unitOfWork.Modules.Delete(id);

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
