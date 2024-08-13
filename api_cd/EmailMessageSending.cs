using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using api_CodeFlow.Model;
using System.Text;

namespace api_CodeFlow
{
    public class EmailMessageSending
    {
        public void SendMailSupportAnswer(string user_email, string user_name)
        {
            string fromEmail = AuthOptions.MyEmail;
            string password = AuthOptions.MyPassword;

            string toEmail = user_email;


            string SubjectMessege = "Служба поддержки CodeFlow"; //тема
            string BodyMessege = $"Добрый день, {user_name}!\r\n\r\n" +
                "Все представители службы поддержки команды CodeFlow с радостью приветствуют Вас!\r\n" +
                "Благодарим за обращение в нашу в службу поддержки!\r\n" +
                "Мы получили Ваш запрос и вышлем Вам полный ответ в течение 48 часов.\r\n"; //тело

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage(fromEmail, toEmail)
            {
                Subject = SubjectMessege,
                Body = BodyMessege
            };

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Отправлено");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Очистка ресурсов
                mailMessage.Dispose();
            }
        }


        public void SendMailRegistration(string user_email, string user_name)
        {
            string fromEmail = AuthOptions.MyEmail;
            string password = AuthOptions.MyPassword;

            string toEmail = user_email;


            string SubjectMessege = "CodeFlow"; //тема
            string BodyMessege = $"Добрый день, {user_name}!\r\n\r\n" +
                "Поздравляем! Вы успешно зарегистрировались.";

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage(fromEmail, toEmail)
            {
                Subject = SubjectMessege,
                Body = BodyMessege
            };

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Отправлено");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Очистка ресурсов
                mailMessage.Dispose();
            }
        }

        public async Task SendMailPurchaseInfo(User user, List<Product> products, Purchase purchase)
        {
            string fromEmail = AuthOptions.MyEmail;
            string password = AuthOptions.MyPassword;
            string toEmail = user.UserEmail;

            decimal total_price = products.Sum(x => x.ProductPrice);

            StringBuilder sb = new StringBuilder();
            sb.Append(
                "<table>" +
                "<tr>" +
                "<th>Наименование товара</th>" +
                "<th>Промежуточный итог (руб.)</th>" +
                "</tr>");
            foreach (var product in products)
            {
                sb.Append(
                    "<tr>" +
                    "<td>" + product.ProductName + "</td>" +
                    "<td>" + product.ProductPrice + "</td>" +
                    "</tr>");
            }
            sb.Append("</table>");


            string emailTemplate =
                @"<!DOCTYPE html>
                <html>
                    <head>
                        <style>
                            body {
                                font-family: Arial, sans-serif;
                                color: #444;
                                background-color: #f0f0f0;
                                padding: 20px;
                            }
                            h1 {
                                color: #333;
                            }
                            table {
                                width: 100%;
                                margin: 20px 0;
                                border-collapse: collapse;
                            }
                            th, td {
                                padding: 10px;
                                border-bottom: 1px solid #ddd;
                                text-align: left;
                            }
                            th {
                                background-color: #4CAF50;
                                color: white;
                            }
                            tr:nth-child(even) {
                                background-color: #f2f2f2;
                            }
                        </style>
                    </head>


                    <body>
                        <h1>Здравствуйте, " + user.UserLogin + "!</h1>" +
                         "<p>Благодарим вас за покупку</p>" +
                         "<h1>Чек № " + purchase.PurchasesId + "</h1>" +
                         "<h2>ИНФОРМАЦИЯ О ВАШЕМ ЗАКАЗЕ:</h2>" +
                         "<p>Идентификатор заказа: " + purchase.PurchasesId + "</p>" +
                         "<p>Счет выставлен: " + user.UserEmail + "</p>" +
                         "<p>Дата заказа: " + DateTime.Now + "</p>" +
                         "<p>Источник: CodeFlow</p>" +

                         "<h2>СОДЕРЖИМОЕ ВАШЕГО ЗАКАЗА:</h2>" +

                    sb.ToString() +
                        "<p>ИТОГО: " + total_price + " руб.</p>" +
                    "</body>" +
                "</html>";

            string SubjectMessege = "Ваш чек CodeFlow"; //тема

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(emailTemplate, null, MediaTypeNames.Text.Html);

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage(fromEmail, toEmail)
            {
                Subject = SubjectMessege,
            };
            mailMessage.AlternateViews.Add(htmlView);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"\nID пользователя{purchase.UserId}\nНомер заказа: {purchase.PurchasesId}\nЧек отправлен\n");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Очистка ресурсов
                mailMessage.Dispose();
            }

            return;
        }
    }
}
