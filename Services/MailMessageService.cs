using eLearningApi.Models;
using System.Net.Mail;

namespace eLearningApi.Services;

public static class MailMessageService
{
    public static MailMessage CreateVerificationEmail(Token token, string sender, string recipient)
    {
        MailMessage message = new MailMessage(sender, recipient);
        message.Subject = "Verification Email";

        message.Body = $"Please verify your email. Click the link: http://localhost:5024/signup/verification?token={token.Value}";

        return message;
    }

    public static MailMessage CreateEmailNotExistMessage(string sender, string recipient)
    {
        MailMessage message = new MailMessage(sender, recipient);
        message.Subject = "Email Not Exist In ELearning";
        message.Body = "Your email address was used in our eLearning system, but it seems you haven't registered your email yet. If this is a mistake, you can ignore this message.";

        return message;
    }

    public static MailMessage CreatePasswordResetEmail(Token token, string sender, string recipient)
    {
        MailMessage message = new MailMessage(sender, recipient);
        message.Subject = "Password Reset";
        message.Body = $"You have requested a password reset. Click the link: http://localhost:5024/signup/password?token={token.Value}";

        return message;
    }

    public static bool Send(this MailMessage message, IConfiguration config)
    {
        throw new NotImplementedException();
        SmtpClient smtpClient = new SmtpClient(config["SmtpServer"], Convert.ToInt32(config["SmtpPort"]));
        smtpClient.EnableSsl = true;

        try
        {
            smtpClient.Send(message);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
