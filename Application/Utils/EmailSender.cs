using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities;
using Domain.Enums;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Application.Utils
{
    public class EmailSender
    {
        public static async Task<bool> SendPasswordResetEmail(string toEmail, string resetLink)
        {
            var userName = "GameMkt";
            var emailFrom = "thongsieusao3@gmail.com";
            var password = "dfni ihvq panf lyjc";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Password Reset Request";
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
            <h2>Password Reset</h2>
            <p>You have requested to reset your password. Please click the button below to reset your password:</p>
            <a class='button' href='{resetLink}'>Reset Password</a>
            <p>If you did not request this, please ignore this email.</p>
        </div>
    </body>
</html>
"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
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
        public static async Task<bool> SendProjectConfirmationEmail(string creatorFullname, string creatorEmail, string staffFullname, string staffEmail, string title, DateTime startDate, DateTime endDate, Enum status)
        {
            var userName = "GameMkt";
            var emailFrom = "thongsieusao3@gmail.com";
            var password = "dfni ihvq panf lyjc";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", creatorEmail));
            message.Subject = "GameMkt notification.";
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
            .mailHeader {{
                text-align: center;
                padding: 20px;
                background: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            }}
            .content {{
                text-align: text-end;
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
        <div class='mailHeader'>
            <h2>Project Confirmation</h2>
        </div>
        <div class='content'>
            <div class='details'>
                <p> <strong>Dear {creatorFullname},</strong></p>
                
                <p>We are pleased to inform you that your project <strong>{title}</strong> has been successfully created.</p>
                <p><strong>Project Title:</strong> {title}</p>
                <p><strong>Start Date:</strong> {startDate:yyyy-MM-dd}</p>
                <p><strong>End Date:</strong> {endDate:yyyy-MM-dd}</p>
                <p><strong>Current Status:</strong> {status}</p>
                <p><strong>Assigned Staff:</strong> {staffFullname}</p>
                <p><strong>Staff Email:</strong> {staffEmail}</p>
            </div>
            
            <p>Your project will be monitored by our staff member <strong>{staffFullname}</strong>. Once you have fully fullfilled your project <strong>{title}</strong>, you can ask your project monitor <strong>({staffFullname})</strong> to approve your project and start earning your money.</p>
            <p>If you have any questions or need assistance, please feel free to reach out via your staff email {staffEmail}.</p>
            <p>Best regards,</p>
            <p><strong>GameMkt Developer Team</strong></p>
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
            .reason {{
                text-align: left;
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
        </div>
        <div class='reason'>
            <p><Strong>Reason:</strong> <Italic>{reason}</Italic></p>
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

        public static async Task<bool> SendCode(string email, string code)
        {
            var userName = "GameMkt";
            var emailFrom = "thongsieusao3@gmail.com";
            var password = "dfni ihvq panf lyjc";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Verification Code";
            message.Body = new TextPart("html")
            {
                Text = " <div class=\"container\" style=\"text-align: center\">\r\n "
                + "<img\r\n src=\"https://cdn-icons-png.flaticon.com/512/3617/3617039.png\"\r\n alt=\"image\"\r\n class=\"image\"\r\n style=\"width: 160px; height: 160px;\"\r\n/>"
                + "\r\n<div class=\"h4\" style=\"padding-top: 16px; font-size: 18px;\">Hi</div>\r\n"
                + "<div style=\"padding-top: 16px; font-size: 20px;\">Here is the confirmation code for your online form:</div>\r\n"
                + " <div class=\"code\" style=\"padding-top: 16px; font-size: 50px; font-weight: bold; color: #f57f0e;\"> " + code + " </div>\r\n"
                + "<div style=\"padding-top: 16px; font-size: 18px;\">\r\nAll you have to do is copy the confirmation code and paste it to your\r\n form to complete the email verification process.\r\n</div>\r\n</div>"
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
