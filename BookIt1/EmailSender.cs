using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace BookIt1
{
    public class EmailSender
    {
        public static async Task SendEmailWithPdfAsync(string toEmail, string subject, string body, byte[] pdfAttachment)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("BookIt", "email")); // Update this
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { TextBody = body };

            // Attach PDF
            builder.Attachments.Add("BookingSummary.pdf", pdfAttachment, new ContentType("application", "pdf"));

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false); // Use your SMTP server
            await client.AuthenticateAsync("email", "app password"); // Secure this
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
