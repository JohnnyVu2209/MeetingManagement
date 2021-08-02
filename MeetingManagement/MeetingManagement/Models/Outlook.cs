using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
namespace MeetingManagement.Models
{
    public class Outlook
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }


        public Outlook(string To, string Subject, string Body)
        {
            this.To = To;
            this.Subject = Subject;
            this.Body = Body;
        }

        public void SendMail()
        {
            try
            {
                //SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("VanlangMeeting@outlook.com", "123456XZ");

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;

                //Add mail
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("VanlangMeeting@outlook.com", "MeetingManager-no-reply");
                mail.To.Add(To);
                mail.Subject = Subject;
                mail.Body = Body;
                //Send mail
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}