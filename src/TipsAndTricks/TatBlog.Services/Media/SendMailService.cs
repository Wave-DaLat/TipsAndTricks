using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace TatBlog.Services.Media;

public class MailSettings
{
    public string Mail { get; set; }
    public string DisplayName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}

public class MailContent
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}

public class SendMailService
{
    private readonly string mailSaveFolder = Path.Combine(Environment.CurrentDirectory, "mailssave");
    private readonly MailSettings mailSettings;

    private readonly ILogger<SendMailService> logger;

    public SendMailService(IOptions<MailSettings> _mailSettings, ILogger<SendMailService> _logger)
    {
        mailSettings = _mailSettings.Value;
        logger = _logger;
        logger.LogInformation("Create SendMailService");
    }

    public async Task SendEmailAsync(MailContent mailContent)
    {
        var message = new MimeMessage();
        message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
        message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
        message.To.Add(MailboxAddress.Parse(mailContent.To));
        message.Subject = mailContent.Subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = mailContent.Body;
        message.Body = builder.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
            await smtp.SendAsync(message);
        }
        catch (Exception ex)
        {
            System.IO.Directory.CreateDirectory(mailSaveFolder);
            var emailsavefile = string.Format(@$"{mailSaveFolder}/{Guid.NewGuid()}.eml");
            await message.WriteToAsync(emailsavefile);

            logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
            logger.LogError(ex.Message);
        }

        smtp.Disconnect(true);

        logger.LogInformation("send mail to: " + mailContent.To);
    }
}