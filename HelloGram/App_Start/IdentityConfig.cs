using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using HelloGram.Core.Models;
using HelloGram.Persistence;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace HelloGram
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            //await ConfigureSendGridAsync(message);
            await SendEmail(message);
        }

        //public AlternateView GetAlternateView(string htmlBody)
        //{
        //    var avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

        //    var inline = new LinkedResource("hellogram_icon.png", MediaTypeNames.Image.Jpeg)
        //    {
        //        ContentId = "#img#"
        //    };
        //    avHtml.LinkedResources.Add(inline);
        //    return avHtml;
        //}

        public async Task SendEmail(IdentityMessage message)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    var email = ConfigurationManager.AppSettings["email"];
                    var password = ConfigurationManager.AppSettings["password"];

                    var loginInfo = new NetworkCredential(email, password);

                    mail.From = new MailAddress(email);
                    mail.To.Add(new MailAddress(message.Destination));
                    mail.Subject = message.Subject;
                    mail.Body = message.Body;
                    mail.IsBodyHtml = true;

                    //mail.AlternateViews.Add(GetAlternateView(message.Body));

                    try
                    {
                        using (var smtpClient = new SmtpClient(ConfigurationManager.AppSettings["outlook-smtp"], Convert.ToInt32(ConfigurationManager.AppSettings["outlook-port"])))
                        {
                            smtpClient.EnableSsl = true;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = loginInfo;
                            await smtpClient.SendMailAsync(mail);
                        }
                    }
                    finally
                    {
                        //dispose the client
                        mail.Dispose();
                    }
                }
            }
            catch (SmtpFailedRecipientsException ex)
            {
                foreach (var t in ex.InnerExceptions)
                {
                    var status = t.StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        throw new ArgumentException("Delivery failed - retrying in 5 seconds.");
                    }
                    else
                    {
                        throw new ArgumentException($"Failed to deliver message to {t.FailedRecipient}");
                    }
                }
            }
            catch (SmtpException e)
            {
                // handle exception here
                throw new ArgumentException(e.ToString());
            }

            catch (Exception ex)
            {
                throw new ArgumentException(ex.ToString());
            }

        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private static void ConfigureSendGridAsync(IdentityMessage message)
        {
            //var client = new SendGridClient(ConfigurationManager.AppSettings["SendGridApiKey"]);
            //var myMessage = new SendGridMessage
            //{
            //    From = new EmailAddress("himato.gamal120@outlook.com", "HelloGram Team"),
            //    Subject = message.Subject,
            //    HtmlContent = message.Body
            //};
            //myMessage.AddTo(message.Destination);
            //myMessage.SetClickTracking(false, false);

            //// Send the email.
            //await client.SendEmailAsync(myMessage);

            //if (transportWeb != null)
            //{
            //    await transportWeb.DeliverAsync(myMessage);
            //}
            //else
            //{
            //    Trace.TraceError("Failed to create Web transport.");
            //    await Task.FromResult(0);
            //}
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //// Plug in your SMS service here to send a text message.
            //// Twilio Begin
            //var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
            //var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
            //var fromNumber = ConfigurationManager.AppSettings["SMSAccountFrom"];

            //TwilioClient.Init(accountSid, authToken);

            //var result = MessageResource.Create(new PhoneNumber(message.Destination), from: new PhoneNumber(fromNumber), body: message.Body);

            ////Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            //Trace.TraceInformation(result.Status.ToString());
            ////Twilio doesn't currently have an async API, so return success.
            //return Task.FromResult(0);
            //// Twilio End

            // ASPSMS Begin 
            //            var soapSms = new ASPSMSX2.ASPSMSX2SoapClient("ASPSMSX2Soap");
            //            soapSms.SendSimpleTextSMS(ConfigurationManager.AppSettings["SMSAccountIdentification"],
            //                    ConfigurationManager.AppSettings["SMSAccountPassword"],
            //                    message.Destination,
            //                    ConfigurationManager.AppSettings["SMSAccountFrom"],
            //                    message.Body);
            //            soapSms.Close();
            //            return Task.FromResult(0);
            // ASPSMS End
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
