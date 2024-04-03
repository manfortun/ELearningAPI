using AutoMapper;
using eLearningApi.DataAccess;
using eLearningApi.DTOs;
using eLearningApi.Models;
using eLearningApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace eLearningApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SignUpController : ErrorHandlingController
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public SignUpController(
        AppDbContext context,
        IMapper mapper,
        IConfiguration config)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
        _config = config;
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult SignUp(UserCreateDto userCreateDto)
    {
        try
        {
            // Ensure that the email can be registered
            if (!UserService.CanRegisterEmail(_unitOfWork.Users, userCreateDto.Email))
            {
                return BadRequest("The email address already exists in the database.");
            }

            var user = _mapper.Map<User>(userCreateDto);

            // set the password and salt
            UserService.SetUserSecurity(user);

            var entityEntry = _unitOfWork.Users.Insert(user);

            if (entityEntry.State == EntityState.Added)
            {
                _unitOfWork.Save();
                var userCompleted = _mapper.Map<UserCreateCompleteDto>(entityEntry.Entity);

                return Ok(userCompleted);
            }

            return BadRequest("Failed to save the account to the database.");
        }
        catch
        {
            return InternalError();
        }
    }

    [AllowAnonymous]
    [HttpPost("verification")]
    public IActionResult SendVerificationEmail(string email)
    {
        MailMessage mailMessage;
        string senderEmail = _config["SenderEmail"] ?? throw new InvalidOperationException("No sender email in application settings.");

        // check if the email exists in the database
        string normalizedEmail = email.ToLower();
        if (_unitOfWork.Users.Get(u => u.Email.ToLower() == normalizedEmail)?.Any() != true)
        {
            mailMessage = MailMessageService.CreateEmailNotExistMessage(senderEmail, email);
        }
        else
        {
            // generate token
            Token token = TokenService.CreateEmailVerificationToken(email);

            _unitOfWork.Tokens.Insert(token);
            _unitOfWork.Save();

            mailMessage = MailMessageService.CreateVerificationEmail(token, senderEmail, email);
        }

        if (mailMessage.Send(_config))
        {
            return Ok();
        }
        else
        {
            return InternalError();
        }
    }

    [AllowAnonymous]
    [HttpGet("verification")]
    public IActionResult GetVerificationStatus(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest();
        }

        // check if token is valid
        Token? tokenObj = _unitOfWork.Tokens.GetToken(token);
        if (tokenObj is null)
        {
            return BadRequest();
        }

        // check if token has been used
        if (tokenObj.IsExecuted)
        {
            return Ok();
        }

        // check if token expiry (24 hours)
        if (DateTime.UtcNow.Subtract(tokenObj.CreatedAt).TotalHours > 24)
        {
            return BadRequest();
        }

        string email = tokenObj.Email.ToLower();
        User? user = _unitOfWork.Users.Get(u => u.Email.ToLower() == email)?.FirstOrDefault();

        if (user is null)
        {
            return BadRequest();
        }

        user.IsActive = true;
        var entityEntry = _unitOfWork.Users.Update(user);

        if (entityEntry.State == EntityState.Modified)
        {
            tokenObj.IsExecuted = true;
            _unitOfWork.Tokens.Update(tokenObj);
        }

        _unitOfWork.Save();
        return Ok();
    }
}
