namespace Core.Utils.Mail;

public class MailSendModel
{
    public string? SenderEmailAddress { get; set; }
    public List<string>? RecipientEmailList { get; set; }
    public string? Subject { get; set; }
    public string? HtmlContent { get; set; }
}
