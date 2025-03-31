using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Utils
{
    public class EmailSender
    {
        public static async Task<bool> SendConfirmationEmail(
            string toEmail,
            string confirmationLink
        )
        {
            var userName = "GameMkt";
            var emailFrom = "thongsieusao3@gmail.com";
            var password = "dfni ihvq panf lyjc";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Confirmation your email to login";
            message.Body = new TextPart("html")
            {
                Text =
        @"
<html>
    <head>
        <style>
            body {
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                margin: 0;
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
            }
            .content {
                text-align: center;
                padding: 20px;
                background: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            }
            .button {
                display: inline-block;
                padding: 12px 24px;
                background: linear-gradient(45deg, #007bff, #0056b3);
                color: #ffffff;
                text-decoration: none;
                border-radius: 8px;
                font-size: 18px;
                font-weight: bold;
                transition: background 0.3s ease-in-out, transform 0.2s;
            }
            .button:hover {
                background: #ffffff;
                transform: scale(1.05);
            }
        </style>
    </head>
    <body>
        <div class='content'>
            <h2>Email Confirmation</h2>
            <p>Please click the button below to confirm your email:</p>                    
            <a class='button' href='" + confirmationLink + @"'>Confirm Email</a>
        </div>
    </body>
</html>
"
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //authenticate account email
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public static async Task<bool> SendHaltedProjectStatusEmailToMonitor(
    string toEmail,
    string projectTitle,
    int projectId,
    bool isSuccessful
)
        {
            var userName = "GameMkt";
            var emailFrom = "thongsieusao3@gmail.com";
            var password = "dfni ihvq panf lyjc";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "New Monitored Project Halted Relatively Recently";
            message.Body = new TextPart("html")
            {
                Text =
        $@"
<html>
    <head>
        <style>
            body {{
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                margin: 0;
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
            }}
            .content {{
                text-align: center;
                padding: 20px;
                background: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            }}
            .button {{
                display: inline-block;
                padding: 12px 24px;
                background: linear-gradient(45deg, #007bff, #0056b3);
                color: #ffffff;
                text-decoration: none;
                border-radius: 8px;
                font-size: 18px;
                font-weight: bold;
                transition: background 0.3s ease-in-out, transform 0.2s;
            }}
            .button:hover {{
                background: #ffffff;
                transform: scale(1.05);
            }}
        </style>
    </head>
    <body>
        <div class='content'>
            <h2>Crowdfunding {(isSuccessful ? "Success!" : "Failure!")}</h2>
            <p>One of your monitored projects, project <strong>{projectTitle}</strong> (Id: {projectId}) has been {(isSuccessful ? "successful" : "unsuccessful")} in gathering enough funding. Any pledged donation should  {(isSuccessful ? "be transferred to the creator's Paypal account based on the account's email." : "be refunded.")}.</p>
            <p>With regards, GameMkt</p>
        </div>
    </body>
</html>
"
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //authenticate account email
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }


        public static async Task<bool> SendHaltedProjectStatusEmailToCreator(
            string toEmail,
            string projectTitle,
            bool isSuccessful
        )
        {
            var userName = "GameMkt";
            var emailFrom = "thongsieusao3@gmail.com";
            var password = "dfni ihvq panf lyjc";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = isSuccessful ? "Crowdfunding Success" : "Crowdfunding Failure";
            message.Body = new TextPart("html")
            {
                Text =
        $@"
<html>
    <head>
        <style>
            body {{
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                margin: 0;
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
            }}
            .content {{
                text-align: center;
                padding: 20px;
                background: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            }}
            .button {{
                display: inline-block;
                padding: 12px 24px;
                background: linear-gradient(45deg, #007bff, #0056b3);
                color: #ffffff;
                text-decoration: none;
                border-radius: 8px;
                font-size: 18px;
                font-weight: bold;
                transition: background 0.3s ease-in-out, transform 0.2s;
            }}
            .button:hover {{
                background: #ffffff;
                transform: scale(1.05);
            }}
        </style>
    </head>
    <body>
        <div class='content'>
            <h2>Crowdfunding {(isSuccessful ? "Success!" : "Failure!")}</h2>
            <p>Your project <strong>{projectTitle}</strong> has been {(isSuccessful ? "successful" : "unsuccessful")} in gathering enough funding. Any pledged donation should  {(isSuccessful ? "be transferred to your Paypal account based on your email." : "be refunded.")}.</p>
            <p>With regards, GameMkt</p>
        </div>
    </body>
</html>
"
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //authenticate account email
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public static async Task<bool> SendProjectResponseEmail(
            string toEmail,
            string projectTitle,
            ProjectEnum projectStatus,
            string reason
        )
        {
            var userName = "GameMkt";
            var emailFrom = "thongsieusao3@gmail.com";
            var password = "dfni ihvq panf lyjc";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = $"Notification about project {projectTitle}";
            message.Body = new TextPart("html")
            {
                Text =
        $@"
<html>
    <head>
        <style>
            body {{
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                margin: 0;
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
            }}
            .content {{
                text-align: center;
                padding: 20px;
                background: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            }}
            .button {{
                display: inline-block;
                padding: 12px 24px;
                background: linear-gradient(45deg, #007bff, #0056b3);
                color: #ffffff;
                text-decoration: none;
                border-radius: 8px;
                font-size: 18px;
                font-weight: bold;
                transition: background 0.3s ease-in-out, transform 0.2s;
            }}
            .button:hover {{
                background: #ffffff;
                transform: scale(1.05);
            }}
        </style>
    </head>
    <body>
        <div class='content'>
            <h2>Project {projectStatus}</h2>
            <p>Your project <strong>{projectTitle}</strong> has been set {projectStatus}.</p>
            <p>Reason: {reason}</p>
        </div>
    </body>
</html>
"
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //authenticate account email
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }
}
