using Busines.Utilities.Contants;
using Business.Abstract;
using Business.Dtos.Contact;
using Business.Helpers.Settings;
using Core.Result.Abstract;
using Core.Result.Concrete;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Business.Concrete
{
    public class EmailManager : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailManager(IOptions< EmailSettings >emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

       

        public async Task<IResult> SentContactEmailAsync(ContactSendDto dto)
        {

            try
            {
                var bodyBuilder = new StringBuilder();
                bodyBuilder.AppendLine("Yeni Contact Form Mesajı");
                bodyBuilder.AppendLine("--------------------------------");
                bodyBuilder.AppendLine($"Ad: {dto.Name}");
                bodyBuilder.AppendLine($"Email: {dto.Email}");
                bodyBuilder.AppendLine($"Mövzu: {dto.Subject}");
                bodyBuilder.AppendLine("Mesaj:");
                bodyBuilder.AppendLine(dto.Message);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = $"Contact Form | {dto.Subject}",
                    Body = bodyBuilder.ToString(),
                    IsBodyHtml = false
                };

                mailMessage.To.Add(_emailSettings.ReceiverEmail);
                mailMessage.ReplyToList.Add(new MailAddress(dto.Email, dto.Name));
                using var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password),
                    EnableSsl = _emailSettings.EnableSsl
                };

                await smtpClient.SendMailAsync(mailMessage);
                return new SuccessResult("Mesaginiz ugurla gonderildi");

            }
            catch(Exception ex)
            {
                return new ErrorResult($"Xəta baş verdi: {ex.Message}");
            }
           
        }

    }
}
