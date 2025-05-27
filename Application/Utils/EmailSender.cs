using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Enums;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Application.Utils
{
    public class EmailSender
    {
        private static (string UserName, string EmailFrom, string Password) GetEmailCredentials()
        {
            return ("GameMkt", "thongsieusao3@gmail.com", "dfni ihvq panf lyjc");
        }

        public static async Task<bool> SendProjectSubmissionEmail(string staffFullName, string staffEmail, decimal minimumAmount, string title, DateTime startDate, DateTime endDate, ProjectStatusEnum projectStatus, int projectId, string note)
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", staffEmail));
            message.Subject = "GameMkt - Project " + title + "In Need Of Approval";
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
        </style>
    </head>
    <body>
        <div class='mailHeader'>
            <h2>Your Invoice Is Here</h2>
        </div>
        <div class='content'>
            <div class='details'>
                <p> <strong>Dear {staffFullName},</strong></p>
                
                <p>A project has been submitted and is pending your approval.</p>
                <p><strong>Project Title:</strong> {title}</p>
                <p><strong>Project Url:</strong> https://game-mkt.vercel.app/project/{projectId}</p>
                <p><strong>Start Date:</strong> {startDate:yyyy-MM-dd}</p>
                <p><strong>End Date:</strong> {endDate:yyyy-MM-dd}</p>
                <p><strong>Status:</strong> {projectStatus}</p>
                <p><strong>Minimum Amount:</strong> ${minimumAmount:F2}</p>
                <p><strong>Note:</strong> ${note}</p>

                <p>Remember to read through and understand any note sent by the project's campaigner. Conduct the most possibly comprehensive examination of the submitted project. Make sure the minimum goal, the campaign timeline and the scope and contents set out for the project are within reason and relevant terms of service. You are to apply circumspection to any decision to approve or reject the project. Rejection is always the safe option. Blunders have consequences.</p>
            </div>

            <p>Best regards,</p>
            <p><strong>GameMkt</strong></p>
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


        public static async Task<bool> SendTransferInvoiceEmail(string creatorFullname, string creatorEmail, decimal amount, string title, string url, DateTime startDate, DateTime endDate, ProjectStatusEnum projectStatus, int projectId)
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", creatorEmail));
            message.Subject = "GameMkt - You've Received Funds of Project " + title;
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
        </style>
    </head>
    <body>
        <div class='mailHeader'>
            <h2>Your Invoice Is Here</h2>
        </div>
        <div class='content'>
            <div class='details'>
                <p> <strong>Dear {creatorFullname},</strong></p>
                
                <p>Funds raised for your project <strong>{title}</strong> have been sent to your registered Paypal account.</p>
                <p><strong>Project Title:</strong> {title}</p>
                <p><strong>Project Url:</strong> https://game-mkt.vercel.app/project/{projectId}</p>
                <p><strong>Start Date:</strong> {startDate:yyyy-MM-dd}</p>
                <p><strong>End Date:</strong> {endDate:yyyy-MM-dd}</p>
                <p><strong>Status:</strong> {projectStatus}</p>
                <p><strong>Transferred Amount:</strong> ${amount:F2}</p>
                <p><strong>Invoice Url:</strong> {url}</p>

                <p>To see the details of the above transaction, you can navigate to https://game-mkt.vercel.app/pledges. Should any issue arise, a report may be filed via the link https://game-mkt.vercel.app/create-report.</p>
            </div>

            <p>Best regards,</p>
            <p><strong>GameMkt</strong></p>
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


        public static async Task<bool> SendMonitorChangeEmail(string creatorFullname, string creatorEmail, string staffFullname, string staffEmail, string title, DateTime startDate, DateTime endDate, ProjectStatusEnum projectStatus, int projectId)
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", creatorEmail));
            message.Subject = "GameMkt - Newly Designated Monitor of Project " + title;
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
        </style>
    </head>
    <body>
        <div class='mailHeader'>
            <h2>New Project Monitor</h2>
        </div>
        <div class='content'>
            <div class='details'>
                <p> <strong>Dear {creatorFullname},</strong></p>
                
                <p>Your project <strong>{title}</strong> has been handed to {staffFullname} for monitoring purposes.</p>
                <p><strong>Project Title:</strong> {title}</p>
                <p><strong>Project Url:</strong> https://game-mkt.vercel.app/project/{projectId}</p>
                <p><strong>Start Date:</strong> {startDate:yyyy-MM-dd}</p>
                <p><strong>End Date:</strong> {endDate:yyyy-MM-dd}</p>
                <p><strong>Status:</strong> {projectStatus}</p>
                <p><strong>Staff:</strong> {staffFullname}</p>
                <p><strong>Staff Email:</strong> {staffEmail}</p>
            </div>

            <p>If you have any questions or need assistance, please feel free to reach out via your monitor's email {staffEmail}.</p>
            <p>Best regards,</p>
            <p><strong>GameMkt</strong></p>
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


        public static async Task<bool> SendMonitorAssignmentEmail(string creatorFullname, string creatorEmail, string staffFullname, string staffEmail, string title, DateTime startDate, DateTime endDate, ProjectStatusEnum projectStatus, int projectId)
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", staffEmail));
            message.Subject = "GameMkt - New Monitoring Assignment To Project " + title;
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
        </style>
    </head>
    <body>
        <div class='mailHeader'>
            <h2>New Monitoring Designation</h2>
        </div>
        <div class='content'>
            <div class='details'>
                <p> <strong>Dear {staffFullname},</strong></p>
                
                <p>As part of the staff of GameMkt, you have been tasked to oversee the project <strong>{title}</strong>.</p>
                <p><strong>Project Title:</strong> {title}</p>
                <p><strong>Project Url:</strong> https://game-mkt.vercel.app/project/{projectId}</p>
                <p><strong>Start Date:</strong> {startDate:yyyy-MM-dd}</p>
                <p><strong>End Date:</strong> {endDate:yyyy-MM-dd}</p>
                <p><strong>Status:</strong> {projectStatus}</p>
                <p><strong>Creator:</strong> {creatorFullname}</p>
                <p><strong>Creator Email:</strong> {creatorEmail}</p>
            </div>
            
            <p>The creator <strong>{staffFullname}</strong> and potentially fellow collaborators may seek your advice and approval via emails or other means of contact. It is higly advisable that you do not overstep boundaries. Cautiously exercise your administrative privileges to the best of your ability and ensure the project and its crowfunding process are perfectly moderated.</p>
            <p>You may contact the game's creator and primary campainger via the email {creatorEmail}.</p>
            <p>Best regards,</p>
            <p><strong>GameMkt</strong></p>
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

        public static async Task<bool> SendRefundInvoiceEmail(string toEmail, string projectTitle, decimal amount, string invoiceUrl, int projectId)
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Billing Receipt for Your Pledge";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
<html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}
            .content {{
                max-width: 600px;
                margin: 20px auto;
                padding: 20px;
                background: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            }}
            .header {{
                text-align: center;
                margin-bottom: 20px;
            }}
            .details {{
                margin-top: 20px;
            }}
            .footer {{
                margin-top: 30px;
                text-align: center;
                font-size: 12px;
                color: #888;
            }}
        </style>
    </head>
    <body>
        <div class='content'>
            <div class='header'>
                <h2>Here's Your Refund Invoice</h2>
            </div>
            <p>Dear Supporter,</p>
            <p>Your pledges to the project <strong>{projectTitle}</strong> have been refunded.</p>
            <div class='details'>
                <p><strong>Project Title:</strong> {projectTitle}</p>
                <p><strong>Project Url:</strong> https://game-mkt.vercel.app/project/{projectId}</p>
                <p><strong>Invoice Url:</strong> {invoiceUrl}</p>
                <p><strong>Refunded Amount:</strong> ${amount:F2}</p>
            </div>
            <p>To see the details of this refund, you can navigate to https://game-mkt.vercel.app/pledges. Should any issue arise, a report may be filed via the link https://game-mkt.vercel.app/create-report.</p>
            <p>Your support is greatly appreciated. If you have any questions, feel free to contact us.</p>
            <div class='footer'>
                <p>GameMkt Team</p>
            </div>
        </div>
    </body>
</html>"
            };


            message.Body = bodyBuilder.ToMessageBody();

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


        public static async Task<bool> SendBillingEmail(string toEmail, string projectTitle, decimal amount, string invoiceUrl, int projectId)
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Billing Receipt for Your Pledge";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
<html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}
            .content {{
                max-width: 600px;
                margin: 20px auto;
                padding: 20px;
                background: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            }}
            .header {{
                text-align: center;
                margin-bottom: 20px;
            }}
            .details {{
                margin-top: 20px;
            }}
            .footer {{
                margin-top: 30px;
                text-align: center;
                font-size: 12px;
                color: #888;
            }}
        </style>
    </head>
    <body>
        <div class='content'>
            <div class='header'>
                <h2>Thank You for Your Pledge!</h2>
            </div>
            <p>Dear Supporter,</p>
            <p>We are pleased to confirm your pledge for the project <strong>{projectTitle}</strong>.</p>
            <div class='details'>
                <p><strong>Project Title:</strong> {projectTitle}</p>
                <p><strong>Project Url:</strong> https://game-mkt.vercel.app/project/{projectId}</p>
                <p><strong>Invoice URL:</strong> {invoiceUrl}</p>
                <p><strong>Amount:</strong> ${amount:F2}</p>
            </div>
            <p>To see your pledges, you can navigate to https://game-mkt.vercel.app/pledges.</p>
            <p>Your support is greatly appreciated. If you have any questions, feel free to contact us.</p>
            <div class='footer'>
                <p>GameMkt Team</p>
            </div>
        </div>
    </body>
</html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

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

        public static async Task<bool> SendPayPalLoginEmailToBacker(string toEmail, string projectTitle, decimal amount, string paymentAccount, string payoutBatchId, int projectId)
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = $"PAYPAL LOGIN REQUEST: GameMkt has commenced a refund for the project {"\"" + projectTitle + "\""}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
<html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}
            .content {{
                max-width: 600px;
                margin: 20px auto;
                padding: 20px;
                background: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            }}
            .header {{
                text-align: center;
                margin-bottom: 20px;
            }}
            .details {{
                margin-top: 20px;
            }}
            .footer {{
                margin-top: 30px;
                text-align: center;
                font-size: 12px;
                color: #888;
            }}
        </style>
    </head>
    <body>
        <div class='content'>
            <div class='header'>
                <h2>Initiated Payout in Paypal By GameMkt</h2>
            </div>
            <p>Dear User,</p>
            <p>An attempt has been made to send an amount of money to your existing Paypal account <strong>{"\"" + paymentAccount + "\""}</strong> from the platform GameMkt as compensation for your apparent pledge to the project <strong>{"\"" + projectTitle + "\""}</strong>.</p>
            <div class='details'>
                <p><strong>Project Title:</strong> {projectTitle}</p>
                <p><strong>Project Url:</strong> https://game-mkt.vercel.app/project/{projectId}</p>
                <p><strong>Payout ID:</strong> {payoutBatchId}</p>
                <p><strong>Amount:</strong> ${amount:F2}</p>
            </div>
            <p>It it in your best interests that you are thereby requested to <strong>LOG IN</strong> to your Paypal account in order to ensure the payout has been successfully processed and completed. Any pledge to a project requires at the time of the pledge your understanding of and consent to GameMkt's platform fee which is not refundable. Receipts provided by GameMkt may not be immediately up-to-date. Apologies for any inconvenience.</p>
            <div class='footer'>
                <p>GameMkt Team</p>
            </div>
        </div>
    </body>
</html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

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

        public static async Task<bool> SendPayPalLoginEmailToCreator(string toEmail, string projectTitle, decimal amount, string paymentAccount, string payoutBatchId, int projectId)
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = $"PAYPAL LOGIN REQUEST: GameMkt has commenced a transaction for the project {"\"" + projectTitle + "\""}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
<html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}
            .content {{
                max-width: 600px;
                margin: 20px auto;
                padding: 20px;
                background: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            }}
            .header {{
                text-align: center;
                margin-bottom: 20px;
            }}
            .details {{
                margin-top: 20px;
            }}
            .footer {{
                margin-top: 30px;
                text-align: center;
                font-size: 12px;
                color: #888;
            }}
        </style>
    </head>
    <body>
        <div class='content'>
            <div class='header'>
                <h2>Initiated Payout in Paypal By GameMkt</h2>
            </div>
            <p>Dear Campaingner and Creator,</p>
            <p>An attempt has been made to send an amount of money to your existing Paypal account <strong>{"\"" + paymentAccount + "\""}</strong> from the platform GameMkt as your project <strong>{"\"" + projectTitle + "\""}</strong> has reached its goal.</p>
            <div class='details'>
                <p><strong>Project Title:</strong> {projectTitle}</p>
                <p><strong>Project Url:</strong> https://game-mkt.vercel.app/project/{projectId}</p>
                <p><strong>Payout ID:</strong> {payoutBatchId}</p>
                <p><strong>Amount:</strong> ${amount:F2}</p>
            </div>
            <p>It it in your best interests that you are thereby requested to <strong>LOG IN</strong> to your Paypal account in order to ensure the payout has been successfully processed. Receipts provided by GameMkt may not be immediately up-to-date. Apologies for any inconvenience.</p>
            <div class='footer'>
                <p>GameMkt Team</p>
            </div>
        </div>
    </body>
</html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

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

        public static async Task<bool> SendPasswordResetEmail(string toEmail, string resetLink)
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

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
            var (userName, emailFrom, password) = GetEmailCredentials();

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
            var (userName, emailFrom, password) = GetEmailCredentials();

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
                
                <p>We are pleased to inform you that your project <strong>{title}</strong> has been successfully submitted.</p>
                <p><strong>Project Title:</strong> {title}</p>
                <p><strong>Start Date:</strong> {startDate:yyyy-MM-dd}</p>
                <p><strong>End Date:</strong> {endDate:yyyy-MM-dd}</p>
                <p><strong>Current Status:</strong> {status}</p>
                <p><strong>Assigned Staff:</strong> {staffFullname}</p>
                <p><strong>Staff Email:</strong> {staffEmail}</p>
            </div>
            
            <p>Your project will be monitored by our staff member <strong>{staffFullname}</strong>. Once you have fully fullfilled your project <strong>{title}</strong>, you can wait for your project monitor <strong>({staffFullname})</strong> to approve your project and start earning your money.</p>
            <p>If you have any questions or need assistance, please feel free to reach out via your monitor's email {staffEmail}.</p>
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
            var (userName, emailFrom, password) = GetEmailCredentials();

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
            var (userName, emailFrom, password) = GetEmailCredentials();

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
            string creatorFullname,
            string toEmail,
            string projectTitle,
            ProjectStatusEnum projectStatus,
            string staffFullname,
            string approvedMonitor,
            string reason
        )
        {
            var (userName, emailFrom, password) = GetEmailCredentials();

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
        </style>
    </head>
    <body>
        <div class='content'>
            <h2>Project {projectStatus}</h2>
            <p>Your project <strong>{projectTitle}</strong> has been set to {projectStatus}.</p>
        </div>
        <div class='reason'>
            <p> <strong>Dear {creatorFullname},</strong></p>
            
            <p>We are pleased to inform you that your project <strong>{projectTitle}</strong> has been <strong>{projectStatus}</strong>.</p>
            <p><strong>Project Title:</strong> {projectTitle}</p>
            <p><strong>Current Status:</strong> {projectStatus}</p>
            <p><strong>Assigned Staff:</strong> {staffFullname}</p>
            <p><strong>Project was approved by:</strong> {approvedMonitor}</p>
            <p><Strong>Reason:</strong> <i>{reason}</i></p>
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
