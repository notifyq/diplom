using api_CodeFlow.Model;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace api_CodeFlow.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Model.User>> Post(Dictionary<string, string> data)
        {
            if (dbContext.Context.Users.FirstOrDefault(x => x.UserLogin == data["login"]) != null)
            {
                return Conflict($"Пользователь с логином {data["login"]} уже существует");
            }
            else if (data["login"].Length < 5 || data["password"].Length < 8 || data["email"].Length < 3)
            {
                return BadRequest("Поля недостаточной длины");
            }
            else if (data["login"].Length > 20 || data["password"].Length > 20 || data["email"].Length > 50)
            {
                return BadRequest("Поля превышают требуемую длину");
            }
            try
            {
                MailAddress mailAddress = new MailAddress(data["email"]);
            }
            catch (Exception)
            {

                return BadRequest("Почта не действительна");
            }

            if (dbContext.Context.Users.FirstOrDefault(x => x.UserEmail == data["email"]) != null)
            {
                return Conflict($"Пользователь с почтой {data["email"]} уже существует");
            }

            Model.User new_user = new Model.User()
            {
                UserId = dbContext.Context.Users.Max(x => x.UserId) + 1,
                UserPassword = BCrypt.Net.BCrypt.HashPassword(data["password"]),
                UserLogin = data["login"],
                UserEmail = data["email"],
                UserRole = 1,
                UserStatus = 1,
                UserName = data["login"],
                UserImage = "",
            };

            dbContext.Context.Users.Add(new_user);
            dbContext.Context.SaveChanges();
            SendMail(new_user.UserEmail, new_user.UserName);
            /*return new ObjectResult("user_id: " + new_user.IdUser) { StatusCode = StatusCodes.Status201Created }; */

            new_user.UserLogin = "";
            new_user.UserPassword = "";
            return new ObjectResult(new_user) { StatusCode = StatusCodes.Status201Created };
        }

        void SendMail(string user_email, string user_name)
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
    }
}
