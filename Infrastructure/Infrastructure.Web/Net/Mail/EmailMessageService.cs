using Infrastructure.Core.Logging;
using Infrastructure.Web.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Infrastructure.Web.Net.Mail
{
    public class EmailMessageService 
    {
        private readonly EmailMessageSettingsRecord _emailMessageSettingsRecord;

        public EmailMessageService(
            EmailMessageSettingsRecord emailMessageSettingsRecord
        )
        {
            _emailMessageSettingsRecord = emailMessageSettingsRecord;
            T = NullLocalizer.Instance;
            Logger = LogManager.GetLogger(this.GetType());
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public EmailMessageSettingsRecord GetEmailMessageSettings()
        {
            var emailMessageSettings = _emailMessageSettingsRecord;
            if (emailMessageSettings == null)
            {
                Logger.Warn("email message settings is null, default value will be used.");
                emailMessageSettings = new EmailMessageSettingsRecord
                {
                    // defaul value.
                    Id = 1,
                    Enable = true,

                    Address = "DLYB@126.com",
                    DeliveryMethod = "Network",
                    EnableSsl = false,
                    Host = "smtp.126.com",
                    Port = 25,
                    RequireCredentials = true,
                    UserName = "DLYB",
                    Password = "inno@126.com"
                };
            }
            return emailMessageSettings;
        }
        
        private static MailMessage MailMessagePreprocess(string from, string to, string subject,
            string body, EmailMessageSettingsRecord smtpSettings, SmtpClient smtpClient, string[] cc = null)
        {
            smtpClient.UseDefaultCredentials = !smtpSettings.RequireCredentials;
            if (!smtpClient.UseDefaultCredentials && !String.IsNullOrWhiteSpace(smtpSettings.UserName))
            {
                smtpClient.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
            }

            if (!string.IsNullOrEmpty(smtpSettings.Host))
            {
                smtpClient.Host = smtpSettings.Host;
            }

            smtpClient.Port = smtpSettings.Port;
            smtpClient.EnableSsl = smtpSettings.EnableSsl;
            smtpClient.DeliveryMethod = (SmtpDeliveryMethod)Enum.Parse(typeof(SmtpDeliveryMethod), smtpSettings.DeliveryMethod);

            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(smtpSettings.Address, from);
            mailMessage.To.Add(to);
            if (cc != null)
            {
                foreach (string c in cc)
                {
                    mailMessage.CC.Add(new MailAddress(c));
                }
            }

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
            mailMessage.BodyEncoding = Encoding.UTF8;
            return mailMessage;
        }

        public bool SendMessage(string from, string to, string subject, string body, string[] cc = null)
        {
            try
            {
                if (string.IsNullOrEmpty(to))
                {
                    Logger.Error("Recipient is missing an email address");
                    return false;
                }

                var smtpSettings = GetEmailMessageSettings();
                // can't process emails if the Smtp settings have not yet been set
                if (smtpSettings != null && smtpSettings.Enable)
                {
                    var smtpClient = new SmtpClient();
                    MailMessage mailMessage = MailMessagePreprocess(from, to, subject, body, smtpSettings, smtpClient,cc);

                    try
                    {
                        smtpClient.Send(mailMessage);
                        Logger.Info("Message sent to {0}: {1}", to, body);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Logger.Error( "An unexpected error while sending a message to {0}: {1}",e, to, body);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error( e,"An unexpected error while sending a message to {0}: {1}", to, body);
                throw e;
            }
            return false;
        }

        public void SendMessageAsync(string from, string to, string subject, 
            string body, object userToken,
            SendCompletedEventHandler completed, string[] cc = null)
        {
            try
            {
                if (string.IsNullOrEmpty(to))
                {
                    Logger.Error("Recipient is missing an email address");
                    return;
                }

                var smtpSettings = GetEmailMessageSettings();
                // can't process emails if the Smtp settings have not yet been set
                if (smtpSettings != null && smtpSettings.Enable)
                {
                    var smtpClient = new SmtpClient();
                    MailMessage mailMessage = MailMessagePreprocess(from, to, subject, body, smtpSettings, smtpClient,cc);

                    try
                    {
                        smtpClient.SendCompleted += completed;
                        smtpClient.SendAsync(mailMessage, userToken);
                        Logger.Debug("Message async send to {0}: {1}", to, body);
                    }
                    catch (Exception e)
                    {
                        Logger.Error( e,"An unexpected error while sending a message to {0}: {1}", to, body);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error( e,"An unexpected error while sending a message to {0}: {1}", to, body);
                throw e;
            }
        }
    }
}