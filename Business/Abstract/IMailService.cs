using Core.Utils.Mail;
namespace Business.Abstract;

public interface IMailService
{
    Task SendMailAsync(MailSendModel mailSendModel, CancellationToken cancellationToken = default); 
}