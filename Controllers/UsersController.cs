using AutoMapper;
using eLearningApi.DataAccess;
using eLearningApi.DTOs;
using eLearningApi.Models;
using eLearningApi.UserRolesAndPermissions;
using eLearningApi.UserRolesAndPermissions.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLearningApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ErrorHandlingController
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UsersController(
        AppDbContext context,
        IMapper mapper)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Policy = nameof(Policy.UserPolicyTwo))]
    public IActionResult GetUsers()
    {
        try
        {
            var users = _unitOfWork.Users.Get();

            if (users is null || !users.Any())
            {
                return Ok();
            }

            var userReads = _mapper.Map<IEnumerable<UserReadDto>>(users);

            var response = new GetUsersDto
            {
                Page = 1,
                Data = userReads.ToArray()
            };

            return Ok(response);
        }
        catch
        {
            return InternalError();
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = nameof(Policy.UserPolicyOne))]
    [AccessOwnRecordOnly(Role.Student)]
    public IActionResult GetUserDetails(int id)
    {
        try
        {
            User? user = _unitOfWork.Users.Find(id);

            if (user is null)
            {
                return Ok();
            }

            var userDto = _mapper.Map<UserReadDto>(user);

            return Ok(userDto);
        }
        catch
        {
            return InternalError();
        }
    }
}
