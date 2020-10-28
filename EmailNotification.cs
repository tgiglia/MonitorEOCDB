using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace MonitorEOCDB
{
    class EmailNotification
    {
        public void testEmail(ConfigData cd)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient server = new SmtpClient(cd.emailAddress);
                mail.From = new MailAddress(cd.emailFrom);
                mail.To.Add(cd.emailTo);

                mail.Subject = "Test Email";
                mail.Body = "This is a for testing SMTP mail from Brewster AWS.";

                server.Port = 25;
                server.EnableSsl = false;
                server.Host = cd.emailAddress;
                server.Send(mail);
                Logger.logIt(cd, "testEmail: email sent.");
               
            }
            catch (Exception e)
            {
                Logger.logIt(cd, e.ToString());              
            }
           
        }
        public bool sendEmail(ConfigData cd, String body, String subject)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient server = new SmtpClient(cd.emailAddress);
                mail.From = new MailAddress(cd.emailFrom);
                mail.To.Add(cd.emailTo);

                mail.Subject = subject;
                mail.Body = body;

                server.Port = 25;
                server.EnableSsl = false;
                server.Host = cd.emailAddress;
                server.Send(mail);
            }
            catch (Exception e)
            {
                Logger.logIt(cd,e.ToString());
                
            }
            return true;
        }
    }
}
