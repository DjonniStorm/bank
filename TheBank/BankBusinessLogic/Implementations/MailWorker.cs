using MailKit.Net.Smtp;
using MimeKit;

namespace BankBusinessLogic.Implementations;

public class MailWorker
{
    public void sendYandex()
    {
        try {
            using (var smtp = new SmtpClient()) {
                smtp.Connect("smtp.yandex.ru", 465, true);
                smtp.Authenticate("egoffevgeny@yandex.com", "mpaffjmvyulsdpev");
            

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "Тут отчет";
                bodyBuilder.HtmlBody = "<h1>Тут отчет</h1>";

                var message = new MimeMessage()
                {
                    Subject = "Отчеты четы четы",
                    Body = bodyBuilder.ToMessageBody()
                };

                message.From.Add(new MailboxAddress("Банк", "egoffevgeny@yandex.com"));
                message.To.Add(new MailboxAddress("Клиент", "vfomicev586@gmail.com"));
            
                smtp.Send(message);
                smtp.Disconnect(true);
            }
        }
        catch (Exception ex) 
        { 
            Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
            throw;
        }
    }
}
