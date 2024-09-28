using Fantasy.Shared.Responses;

namespace Fantasy.Backend.Helpers;

public interface IMailHelper
{
    ActionResponse<string> SendMail(string toName, string toEmail, string subject, string body, string language);
}