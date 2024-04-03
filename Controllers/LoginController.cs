using AutoMapper;
using eLearningApi.DataAccess;
using eLearningApi.DTOs;
using eLearningApi.Models;
using eLearningApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLearningApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly TokenManager _tokenManager;

    public LoginController(
        AppDbContext context,
        IMapper mapper,
        IConfiguration config,
        TokenManager tokenManager)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
        _config = config;
        _tokenManager = tokenManager;
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Login(UserLoginDto loginDto)
    {
        // check if the email exists in the database
        string email = loginDto.Email.ToLower();
        User? user = _unitOfWork.Users.Get(u => u.Email.ToLower() == email)?.FirstOrDefault();

        if (user is null)
        {
            return BadRequest("User not exists.");
        }

        // check if the user is verified
        if (!user.IsActive)
        {
            return BadRequest("User not verified.");
        }

        if (!UserService.VerifyPassword(user, loginDto.Password))
        {
            return BadRequest("Invalid password.");
        }

        var userLoginSuccess = _mapper.Map<UserLoginSuccessDto>(user);
        userLoginSuccess.AccessToken = TokenService.GenerateTokenString(_config, user);

        // set token as active
        _tokenManager.ActivateToken(userLoginSuccess.AccessToken);

        return Ok(userLoginSuccess);
    }
}
