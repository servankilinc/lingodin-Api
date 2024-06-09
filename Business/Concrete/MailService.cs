using Azure;
using Azure.Communication.Email;
using Business.Abstract;
using Core.CrossCuttingConcerns;
using Core.Exceptions;
using Core.Utils.Azure;
using Core.Utils.Mail;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Business.Concrete;

[BusinessExceptionHandler]
public class MailService : IMailService
{
    private readonly EmailClient _emailClient;
    private readonly AzureSettings _azureSettings;
    private readonly ILogger<MailService> _loger;
    
    public MailService(EmailClient emailClient, AzureSettings azureSettings,  ILogger<MailService> loger)
    { 
        _emailClient = emailClient;
        _azureSettings = azureSettings;
        _loger = loger;
    }


    public async Task SendMailAsync(MailSendModel mailSendModel, CancellationToken cancellationToken = default)
    {
        EmailMessage message = MessageGenerator(mailSendModel);

        EmailSendOperation emailSendOperation = await _emailClient.SendAsync(WaitUntil.Completed, message, cancellationToken);

        EmailSendResult sendResult = emailSendOperation.Value;

        if (sendResult.Status == EmailSendStatus.Succeeded)
        {
            _loger.LogInformation("Email Sent Successfully");
        }
        else
        {
            throw new BusinessException($"Failed to send email. Status: {sendResult.Status} detail : {JsonSerializer.Serialize(message)}");
        }
    }

    private EmailMessage MessageGenerator(MailSendModel mailSendModel)
    {
        if (mailSendModel.RecipientEmailList == null || mailSendModel.RecipientEmailList.Count == 0) throw new ArgumentNullException("RecipientEmailList required!");

        string senderAddress = string.IsNullOrWhiteSpace(mailSendModel.SenderEmailAddress) == false ? mailSendModel.SenderEmailAddress : _azureSettings.DefaultMailSenderAddress;

        var content = new EmailContent(mailSendModel.Subject);

        if (!string.IsNullOrEmpty(mailSendModel.HtmlContent)) 
            content.Html = mailSendModel.HtmlContent;

        List<EmailAddress> emailAddresses = mailSendModel.RecipientEmailList.Select(e => new EmailAddress(e)).ToList();

        var recipients = new EmailRecipients(emailAddresses);
 
        return new EmailMessage(
            senderAddress: senderAddress,
            content: content,
            recipients: recipients
        ); 
    }
}