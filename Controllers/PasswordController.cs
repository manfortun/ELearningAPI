using eLearningApi.DataAccess;
using eLearningApi.DTOs;
using eLearningApi.Enums;
using eLearningApi.Models;
using eLearningApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eLearningApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PasswordController : ErrorHandlingController
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IConfiguration _config;

    public PasswordController(
        AppDbContext context,
        IConfiguration config)
    {
        _unitOfWork = new UnitOfWork(context);
        _config = config;
    }

    [HttpGet]
    public IActionResult SendResetPassword(string email)
    {
        // check if the email exists in the database
        string loweredEmail = email.ToLower();
        User? user = _unitOfWork.Users.Get(u => u.Email.ToLower() == loweredEmail)?.FirstOrDefault();

        if (user is null)
        {
            return BadRequest();
        }

        string senderEmail = _config["SenderEmail"] ?? throw new InvalidOperationException("No sender email specified in application settings.");

        Token token = TokenService.CreateChangePasswordToken(email);

        var entityEntry = _unitOfWork.Tokens.Insert(token);

        if (entityEntry.State == EntityState.Added)
        {
            _unitOfWork.Save();
        }

        bool result = MailMessageService
            .CreatePasswordResetEmail(token, senderEmail, email)
            .Send(_config);

        if (result)
        {
            return Ok();
        }
        else
        {
            return InternalError();
        }
    }

    [HttpPost]
    public IActionResult ResetPassword(PasswordResetDto passwordReset)
    {
        // check if token is empty
        string? tokenString = TokenService.GetAuthToken(HttpContext);
        if (string.IsNullOrEmpty(tokenString))
        {
            return Unauthorized();
        }

        // check if the token exists in the database
        Token? token = _unitOfWork.Tokens.GetToken(tokenString);
        if (token is null)
        {
            return BadRequest();
        }

        // check if token is of password reset type
        if (token.TokenType != nameof(TokenType.PasswordReset))
        {
            return BadRequest();
        }

        // get user information
        var claims = TokenService.GetClaims(HttpContext);
        string? email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest();
        }

        // check if the user exists in the database
        User? user = _unitOfWork.Users.Get(u => u.Email == email)?.FirstOrDefault();
        if (user is null)
        {
            return BadRequest();
        }

        user.Password = passwordReset.Password;

        try
        {
            var entityEntry = _unitOfWork.Users.Update(user);

            if (entityEntry.State == EntityState.Modified)
            {
                _unitOfWork.Save();
                return Ok("Success");
            }
        }
        catch
        {
            // FALLTHROUGH
        }

        return InternalError();
    }
}
