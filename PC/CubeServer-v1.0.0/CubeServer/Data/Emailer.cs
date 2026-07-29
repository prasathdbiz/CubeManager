using CubeServer.Models;
using CubeServer.Pages;
using MailKit;
using MimeKit;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using System.Drawing.Drawing2D;
using MimeKit.Utils;
using MailKit.Security;
using static QRCoder.PayloadGenerator;
using MudBlazor;

namespace CubeServer.Data
{

    public class MailData
    {
        public string Addressee;
        public string Cid;
        public DateTime date1;
        public DateTime date2;
    }

    public class Emailer
    {
        string server;
        int port;
        string username;
        string userid;
        string password;
        string bcc;

        const bool captureEmail = true; // true for testing
        const bool authRequired = false; // for relay

        public Emailer()
        {

        }

        public void SetSmtpServer(string server, int port, string username, string userid, string password)
        {
            this.server = server;
            this.port = port;
            this.username = username;
            this.userid = userid;
            this.password = password;
        }

        public void SetBcc(string bcc)
        {
            this.bcc = bcc;
        }

        public bool SendDailyReports()
        {
            try
            {
                bool status;
                List<Report> reports = new List<Report>();

                status = Global.db.GetReports(0, "Released", "Daily", reports);
                if (!status)
                {
                    Global.logger.LogMessageEx("Error", "Error emailing daily reports.");
                    return false;
                }

                foreach (Report r in reports)
                {
                    var message = new MimeMessage();

                    message.From.Add(new MailboxAddress(username, userid));
                    if (captureEmail)
                    {
                        message.To.Add(new MailboxAddress(bcc, bcc));
                    }
                    else
                    { 
                        foreach (string em in r.EmailTo)
                        {
                            message.To.Add(new MailboxAddress("", em));
                        }
                        foreach (string em in r.EmailCC)
                        {
                            message.Cc.Add(new MailboxAddress("", em));
                        }
                        message.Bcc.Add(new MailboxAddress("", bcc));
                    }

                    message.Subject = string.Format("{0}: Cube Testing Report for {1}", r.ProjectCode, r.StartDate.ToString("yyyy-MM-dd"));

                    var builder = new BodyBuilder();
                    string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports\\tuvsud_logo_sml.png");
                    var image = builder.LinkedResources.Add(imgFileSrc);
                    image.ContentId = MimeUtils.GenerateMessageId();

                    // generate html
                    string tmplFile = Path.Combine(Global.assyPath, "ScribanReports\\DailyReportEmail.tpl");
                    string tmplContents = File.ReadAllText(tmplFile);
                    Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

                    MailData mailData = new MailData();
                    Project p = Global.db.GetProject(r.ProjectId);
                    if (p == null)
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has unknown Project ID.", r.ProjectId);
                        continue;
                    }

                    mailData.Addressee = p.ApplicantName;
                    mailData.Cid = image.ContentId;
                    if (r.StartDate.HasValue)
                    {
                        mailData.date1 = (DateTime)r.StartDate;
                    }
                    else
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has no test date.", r.Id);
                        continue;
                    }

                    string content = tmpl.Render(new { M = mailData });
                    builder.HtmlBody = content;

                    string longName = string.Format("{0}\\{1}\\{2}", r.ReportType, r.StartDate.ToString("yyyyMMdd"), r.FileName);
                    string attachFile = Path.Combine(Global.reportStoragePath, longName);
                    builder.Attachments.Add(attachFile);

                    message.Body = builder.ToMessageBody();

                    // send email
                    using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                    {
                        //smtp.Connect(server, port, SecureSocketOptions.StartTls);
                        smtp.Connect(server, port, SecureSocketOptions.Auto);
                        if (authRequired) 
                        { 
                            smtp.Authenticate(userid, password); 
                        }
                        smtp.Send(message);
                        smtp.Disconnect(true);
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception sending daily reports: {0}", ex.Message);
                return false;
            }
        }

        public bool SendDailyReport(int projId)
        {
            try
            {
                bool status;
                List<Report> reports = new List<Report>();

                status = Global.db.GetReports(projId, "Released", "Daily", reports);
                if (!status)
                {
                    Global.logger.LogMessageEx("Error", "Error emailing daily reports.");
                    return false;
                }

                foreach (Report r in reports)
                {
                    var message = new MimeMessage();

                    message.From.Add(new MailboxAddress(username, userid));

                    if (captureEmail)
                    {
                        message.To.Add(new MailboxAddress(bcc, bcc));
                    }
                    else
                    {
                        foreach (string em in r.EmailTo)
                        {
                            message.To.Add(new MailboxAddress(em, em));
                        }
                        foreach (string em in r.EmailCC)
                        {
                            message.Cc.Add(new MailboxAddress(em, em));
                        }
                        message.Bcc.Add(new MailboxAddress(bcc, bcc));
                    }

                    message.Subject = string.Format("{0}: Cube Testing Report for {1}", r.ProjectCode, r.StartDate.ToString("yyyy-MM-dd"));

                    var builder = new BodyBuilder();
                    string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports\\tuvsud_logo_sml.png");
                    var image = builder.LinkedResources.Add(imgFileSrc);
                    image.ContentId = MimeUtils.GenerateMessageId();

                    // generate html
                    string tmplFile = Path.Combine(Global.assyPath, "ScribanReports\\DailyReportEmail.tpl");
                    string tmplContents = File.ReadAllText(tmplFile);
                    Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

                    MailData mailData = new MailData();
                    Project p = Global.db.GetProject(r.ProjectId);
                    if (p == null)
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has unknown Project ID.", r.ProjectId);
                        continue;
                    }

                    mailData.Addressee = p.ApplicantName;
                    mailData.Cid = image.ContentId;
                    if (r.StartDate.HasValue)
                    {
                        mailData.date1 = (DateTime)r.StartDate;
                    }
                    else
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has no test date.", r.Id);
                        continue;
                    }

                    string content = tmpl.Render(new { M = mailData });
                    builder.HtmlBody = content;

                    string longName = string.Format("{0}\\{1}\\{2}", r.ReportType, r.StartDate.ToString("yyyyMMdd"), r.FileName);
                    string attachFile = Path.Combine(Global.reportStoragePath, longName);
                    builder.Attachments.Add(attachFile);

                    message.Body = builder.ToMessageBody();

                    // send email
                    using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                    {
                        //smtp.Connect(server, port, SecureSocketOptions.StartTls);
                        smtp.Connect(server, port, SecureSocketOptions.Auto);
                        if (authRequired)
                        {
                            smtp.Authenticate(userid, password);
                        }
                        smtp.Send(message);
                        smtp.Disconnect(true);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception sending daily reports: {0}", ex.Message);
                return false;
            }
        }

        public bool SendDailyReport(ReportState reportState)
        {
            try
            {
                bool status;
                List<Report> reports = new List<Report>();

                status = Global.db.GetReports(reportState.project.Id, "All", "Daily", reports, reportState.date1);
                if (!status)
                {
                    Global.logger.LogMessageEx("Error", "Error emailing daily reports.");
                    return false;
                }

                foreach (Report r in reports)
                {
                    var message = new MimeMessage();

                    message.From.Add(new MailboxAddress(username, userid));

                    if (captureEmail)
                    {
                        message.To.Add(new MailboxAddress(bcc, bcc));
                    }
                    else
                    {
                        foreach (string em in r.EmailTo)
                        {
                            message.To.Add(new MailboxAddress(em, em));
                        }
                        foreach (string em in r.EmailCC)
                        {
                            message.Cc.Add(new MailboxAddress(em, em));
                        }
                        message.Bcc.Add(new MailboxAddress(bcc, bcc));
                    }

                    message.Subject = string.Format("{0}: Cube Testing Report for {1}", r.ProjectCode, r.StartDate.ToString("yyyy-MM-dd"));

                    var builder = new BodyBuilder();
                    string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports\\tuvsud_logo_sml.png");
                    var image = builder.LinkedResources.Add(imgFileSrc);
                    image.ContentId = MimeUtils.GenerateMessageId();

                    // generate html
                    string tmplFile = Path.Combine(Global.assyPath, "ScribanReports\\DailyReportEmail.tpl");
                    string tmplContents = File.ReadAllText(tmplFile);
                    Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

                    MailData mailData = new MailData();
                    Project p = Global.db.GetProject(r.ProjectId);
                    if (p == null)
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has unknown Project ID.", r.ProjectId);
                        continue;
                    }

                    mailData.Addressee = p.ApplicantName;
                    mailData.Cid = image.ContentId;
                    if (r.StartDate.HasValue)
                    {
                        mailData.date1 = (DateTime)r.StartDate;
                    }
                    else
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has no test date.", r.Id);
                        continue;
                    }

                    string content = tmpl.Render(new { M = mailData });
                    builder.HtmlBody = content;

                    string longName = string.Format("{0}\\{1}\\{2}", r.ReportType, r.StartDate.ToString("yyyyMMdd"), r.FileName);
                    string attachFile = Path.Combine(Global.reportStoragePath, longName);
                    builder.Attachments.Add(attachFile);

                    message.Body = builder.ToMessageBody();

                    // send email
                    using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                    {
                        smtp.Connect(server, port, SecureSocketOptions.Auto);
                        if (authRequired)
                        {
                            smtp.Authenticate(userid, password);
                        }
                        smtp.Send(message);
                        smtp.Disconnect(true);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception sending daily reports: {0}", ex.Message);
                return false;
            }
        }

        public bool SendDailyReport(Report r, string user)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(username, userid));
                if (captureEmail)
                {
                    message.To.Add(new MailboxAddress(bcc, bcc));
                }
                else
                {
                    foreach (string em in r.EmailTo)
                    {
                        message.To.Add(new MailboxAddress(em, em));
                    }
                    foreach (string em in r.EmailCC)
                    {
                        message.Cc.Add(new MailboxAddress(em, em));
                    }
                    message.Bcc.Add(new MailboxAddress(bcc, bcc));
                }

                message.Subject = string.Format("{0}: Cube Testing Report for {1}", r.ProjectCode, r.StartDate.ToString("yyyy-MM-dd"));

                var builder = new BodyBuilder();
                string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports\\tuvsud_logo_sml.png");
                var image = builder.LinkedResources.Add(imgFileSrc);
                image.ContentId = MimeUtils.GenerateMessageId();

                // generate html
                string tmplFile = Path.Combine(Global.assyPath, "ScribanReports\\DailyReportEmail.tpl");
                string tmplContents = File.ReadAllText(tmplFile);
                Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

                MailData mailData = new MailData();
                Project p = Global.db.GetProject(r.ProjectId);
                if (p == null)
                {
                    Global.logger.LogMessageEx("Error", "Report {0} has unknown Project ID.", r.ProjectId);
                    return false;
                }

                mailData.Addressee = p.ApplicantName;
                mailData.Cid = image.ContentId;
                if (r.StartDate.HasValue)
                {
                    mailData.date1 = (DateTime)r.StartDate;
                }
                else
                {
                    Global.logger.LogMessageEx("Error", "Report {0} has no test date.", r.Id);
                    return false;
                }

                string content = tmpl.Render(new { M = mailData });
                builder.HtmlBody = content;

                string longName = string.Format("{0}\\{1}\\{2}", r.ReportType, r.StartDate.ToString("yyyyMMdd"), r.FileName);
                string attachFile = Path.Combine(Global.reportStoragePath, longName);
                if (File.Exists(attachFile))
                {
                    builder.Attachments.Add(attachFile);
                }
                else
                {
                    Global.logger.LogMessageEx("Error", "Report {0} missing. Cannot send report.", attachFile);
                    return false;
                }

                message.Body = builder.ToMessageBody();

                // send email
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    //smtp.Connect(server, port, SecureSocketOptions.StartTls);
                    smtp.Connect(server, port, SecureSocketOptions.Auto);
                    if (authRequired)
                    {
                        smtp.Authenticate(userid, password);
                    }
                    smtp.Send(message);
                    smtp.Disconnect(true);
                }

                Global.logger.LogMessageEx("Info", "Sent {0} Report {1} for Project {2}.", r.ReportType, r.Id, r.ProjectCode);

                r.Released = 1;
                r.ReleaseUser = user;
                r.ReleaseDate = DateTime.Now;

                bool status = Global.db.UpdateReport(r, user);
                if (!status)
                {
                    Global.logger.LogMessageEx("Error", "Error updating report after sending {0} Report.", r.ReportType);
                }

                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception sending daily reports: {0}", ex.Message);
                return false;
            }
        }


        public bool SendMonthlyReports()
        {
            try
            {
                bool status;
                List<Report> reports = new List<Report>();

                status = Global.db.GetReports(0, "Released", "Monthly", reports);
                if (!status)
                {
                    Global.logger.LogMessageEx("Error", "Error emailing monthly reports.");
                    return false;
                }

                foreach (Report r in reports)
                {
                    var message = new MimeMessage();

                    message.From.Add(new MailboxAddress(username, userid));
                    if (captureEmail)
                    {
                        message.To.Add(new MailboxAddress(bcc, bcc));
                    }
                    else
                    {
                        foreach (string em in r.EmailTo)
                        {
                            message.To.Add(new MailboxAddress("", em));
                        }
                        foreach (string em in r.EmailCC)
                        {
                            message.Cc.Add(new MailboxAddress("", em));
                        }
                        message.Bcc.Add(new MailboxAddress("", bcc));
                    }

                    message.Subject = string.Format("{0}: Monthly Report for {1} to {2}", r.ProjectCode,
                        r.StartDate.ToString("yyyy-MM-dd"), r.EndDate.ToString("yyyy-MM-dd"));

                    var builder = new BodyBuilder();
                    string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports\\tuvsud_logo_sml.png");
                    var image = builder.LinkedResources.Add(imgFileSrc);
                    image.ContentId = MimeUtils.GenerateMessageId();

                    // generate html
                    string tmplFile = Path.Combine(Global.assyPath, "ScribanReports\\MontlyReportEmail.tpl");
                    string tmplContents = File.ReadAllText(tmplFile);
                    Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

                    MailData mailData = new MailData();
                    Project p = Global.db.GetProject(r.ProjectId);
                    if (p == null)
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has unknown Project ID.", r.ProjectId);
                        continue;
                    }

                    mailData.Addressee = p.ApplicantName;
                    mailData.Cid = image.ContentId;
                    if (r.StartDate.HasValue)
                    {
                        mailData.date1 = (DateTime)r.StartDate;
                    }
                    else
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has no test date.", r.Id);
                        continue;
                    }

                    string content = tmpl.Render(new { M = r });
                    builder.HtmlBody = content;

                    string longName = string.Format("{0}\\{1}\\{2}", r.ReportType, r.StartDate.ToString("yyyyMMdd"), r.FileName);
                    string attachFile = Path.Combine(Global.reportStoragePath, longName);
                    if (File.Exists(attachFile))
                    {
                        builder.Attachments.Add(attachFile);
                    }
                    else
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} missing. Cannot send report.", attachFile);
                        return false;
                    }

                    message.Body = builder.ToMessageBody();

                    // send email
                    using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                    {
                        //smtp.Connect(server, port, SecureSocketOptions.StartTls);
                        smtp.Connect(server, port, SecureSocketOptions.Auto);
                        if (authRequired)
                        {
                            smtp.Authenticate(userid, password);
                        }
                        smtp.Send(message);
                        smtp.Disconnect(true);
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception sending monthly reports: {0}", ex.Message);
                return false;
            }
        }

        public bool SendMonthlyReport(Report r, string user)
        {
            try
            {
                if (r.ReportType != "Monthly")
                {
                    Global.logger.LogMessageEx("Error", "Report {0} is not a Monthly report. Cannot send.", r.Id);
                    return false;
                }                    

                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(username, this.userid));
                if (captureEmail)
                {
                    message.To.Add(new MailboxAddress(bcc, bcc));
                }
                else
                {
                    foreach (string em in r.EmailTo)
                    {
                        message.To.Add(new MailboxAddress(em, em));
                    }
                    foreach (string em in r.EmailCC)
                    {
                        message.Cc.Add(new MailboxAddress(em, em));
                    }
                    message.Bcc.Add(new MailboxAddress(bcc, bcc));
                }

                message.Subject = string.Format("{0}: Monthly Report for {1} to {2}", r.ProjectCode,
                    r.StartDate.ToString("yyyy-MM-dd"), r.EndDate.ToString("yyyy-MM-dd"));

                var builder = new BodyBuilder();
                string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports\\tuvsud_logo_sml.png");
                var image = builder.LinkedResources.Add(imgFileSrc);
                image.ContentId = MimeUtils.GenerateMessageId();

                // generate html
                string tmplFile = Path.Combine(Global.assyPath, "ScribanReports\\MontlyReportEmail.tpl");
                string tmplContents = File.ReadAllText(tmplFile);
                Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

                MailData mailData = new MailData();
                Project p = Global.db.GetProject(r.ProjectId);
                if (p == null)
                {
                    Global.logger.LogMessageEx("Error", "Report {0} has unknown Project ID.", r.ProjectId);
                    return false;
                }

                mailData.Addressee = p.ApplicantName;
                mailData.Cid = image.ContentId;
                if (r.StartDate.HasValue)
                {
                    mailData.date1 = (DateTime)r.StartDate;
                }
                else
                {
                    Global.logger.LogMessageEx("Error", "Report {0} has no test date.", r.Id);
                    return false;
                }

                string content = tmpl.Render(new { M = r });
                builder.HtmlBody = content;

                string longName = string.Format("{0}\\{1}\\{2}", r.ReportType, r.StartDate.ToString("yyyyMMdd"), r.FileName);
                string attachFile = Path.Combine(Global.reportStoragePath, longName);
                if (File.Exists(attachFile))
                {
                    builder.Attachments.Add(attachFile);
                }
                else
                {
                    Global.logger.LogMessageEx("Error", "Report {0} missing. Cannot send report.", attachFile);
                    return false;
                }

                message.Body = builder.ToMessageBody();

                // send email
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    //smtp.Connect(server, port, SecureSocketOptions.StartTls);
                    smtp.Connect(server, port, SecureSocketOptions.Auto);
                    if (authRequired)
                    {
                        smtp.Authenticate(this.userid, password);
                    }
                    smtp.Send(message);
                    smtp.Disconnect(true);
                }

                Global.logger.LogMessageEx("Info", "Sent Monthly Report {0} for Project {1}.", r.Id, r.ProjectCode);

                r.Released = 1;
                r.ReleaseUser = user;
                r.ReleaseDate = DateTime.Now;

                bool status = Global.db.UpdateReport(r, user);
                if (!status)
                {
                    Global.logger.LogMessageEx("Error", "Error updating report after sending monthly report.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception sending monthly reports: {0}", ex.Message);
                return false;
            }
        }

        public bool SendStatisticalReports()
        {
            try
            {
                bool status;
                List<Report> reports = new List<Report>();

                status = Global.db.GetReports(0, "Released", "Statistical", reports);
                if (!status)
                {
                    Global.logger.LogMessageEx("Error", "Error emailing statistical reports.");
                    return false;
                }

                foreach (Report r in reports)
                {
                    var message = new MimeMessage();

                    message.From.Add(new MailboxAddress(username, userid));
                    if (captureEmail)
                    {
                        message.To.Add(new MailboxAddress("", bcc));
                    }
                    else
                    {
                        foreach (string em in r.EmailTo)
                        {
                            message.To.Add(new MailboxAddress("", em));
                        }
                        foreach (string em in r.EmailCC)
                        {
                            message.Cc.Add(new MailboxAddress("", em));
                        }
                        message.Bcc.Add(new MailboxAddress("", bcc));
                    }

                    message.Subject = string.Format("{0}: Monthly Statistical Report for {1} to {2}", r.ProjectCode,
                        r.StartDate.ToString("yyyy-MM-dd"), r.EndDate.ToString("yyyy-MM-dd"));

                    var builder = new BodyBuilder();
                    string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports\\tuvsud_logo_sml.png");
                    var image = builder.LinkedResources.Add(imgFileSrc);
                    image.ContentId = MimeUtils.GenerateMessageId();

                    // generate html
                    string tmplFile = Path.Combine(Global.assyPath, "ScribanReports\\MontlyStatisticalReportEmail.tpl");
                    string tmplContents = File.ReadAllText(tmplFile);
                    Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

                    MailData mailData = new MailData();
                    Project p = Global.db.GetProject(r.ProjectId);
                    if (p == null)
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has unknown Project ID.", r.ProjectId);
                        continue;
                    }

                    mailData.Addressee = p.ApplicantName;
                    mailData.Cid = image.ContentId;
                    if (r.StartDate.HasValue)
                    {
                        mailData.date1 = (DateTime)r.StartDate;
                    }
                    else
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} has no test date.", r.Id);
                        continue;
                    }

                    string content = tmpl.Render(new { M = mailData });
                    builder.HtmlBody = content;

                    string longName = string.Format("{0}\\{1}\\{2}", r.ReportType, r.StartDate.ToString("yyyyMMdd"), r.FileName);
                    string attachFile = Path.Combine(Global.reportStoragePath, longName);
                    if (File.Exists(attachFile))
                    {
                        builder.Attachments.Add(attachFile);
                    }
                    else
                    {
                        Global.logger.LogMessageEx("Error", "Report {0} missing. Cannot send report.", attachFile);
                        return false;
                    }

                    message.Body = builder.ToMessageBody();

                    // send email
                    using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                    {
                        //smtp.Connect(server, port, SecureSocketOptions.StartTls);
                        smtp.Connect(server, port, SecureSocketOptions.Auto);
                        if (authRequired)
                        {
                            smtp.Authenticate(userid, password);
                        }
                        smtp.Send(message);
                        smtp.Disconnect(true);
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception sending monthly statistical reports: {0}", ex.Message);
                return false;
            }
        }

        public bool SendStatisticalReport(Report r, string user)
        {
            try
            {
                if (r.ReportType != "Statistical")
                {
                    Global.logger.LogMessageEx("Error", "Report {0} is not a Statistical report. Cannot send.", r.Id);
                    return false;
                }

                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(username, userid));
                if (captureEmail)
                {
                    message.To.Add(new MailboxAddress("", bcc));
                }
                else
                {
                    foreach (string em in r.EmailTo)
                    {
                        message.To.Add(new MailboxAddress("", em));
                    }
                    foreach (string em in r.EmailCC)
                    {
                        message.Cc.Add(new MailboxAddress("", em));
                    }
                    message.Bcc.Add(new MailboxAddress("", bcc));
                }

                message.Subject = string.Format("{0}: Monthly Statistical Report for {1} to {2}", r.ProjectCode,
                    r.StartDate.ToString("yyyy-MM-dd"), r.EndDate.ToString("yyyy-MM-dd"));

                var builder = new BodyBuilder();
                string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports\\tuvsud_logo_sml.png");
                var image = builder.LinkedResources.Add(imgFileSrc);
                image.ContentId = MimeUtils.GenerateMessageId();

                // generate html
                string tmplFile = Path.Combine(Global.assyPath, "ScribanReports\\MontlyStatisticalReportEmail.tpl");
                string tmplContents = File.ReadAllText(tmplFile);
                Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

                MailData mailData = new MailData();
                Project p = Global.db.GetProject(r.ProjectId);
                if (p == null)
                {
                    Global.logger.LogMessageEx("Error", "Report {0} has unknown Project ID.", r.ProjectId);
                    return false;
                }

                mailData.Addressee = p.ApplicantName;
                mailData.Cid = image.ContentId;
                if (r.StartDate.HasValue)
                {
                    mailData.date1 = (DateTime)r.StartDate;
                }
                else
                {
                    Global.logger.LogMessageEx("Error", "Report {0} has no test date.", r.Id);
                    return false;
                }

                string content = tmpl.Render(new { M = mailData });
                builder.HtmlBody = content;

                string longName = string.Format("{0}\\{1}\\{2}", r.ReportType, r.StartDate.ToString("yyyyMMdd"), r.FileName);
                string attachFile = Path.Combine(Global.reportStoragePath, longName);
                if (File.Exists(attachFile))
                {
                    builder.Attachments.Add(attachFile);
                }
                else
                {
                    Global.logger.LogMessageEx("Error", "Report {0} missing. Cannot send report.", attachFile);
                    return false;
                }

                message.Body = builder.ToMessageBody();

                // send email
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    //smtp.Connect(server, port, SecureSocketOptions.StartTls);
                    smtp.Connect(server, port, SecureSocketOptions.Auto);
                    if (authRequired)
                    {
                        smtp.Authenticate(userid, password);
                    }
                    smtp.Send(message);
                    smtp.Disconnect(true);
                }

                Global.logger.LogMessageEx("Info", "Sent Statistical Report {0} for Project {1}.", r.Id, r.ProjectCode);

                r.Released = 1;
                r.ReleaseUser = user;
                r.ReleaseDate = DateTime.Now;

                bool status = Global.db.UpdateReport(r, user);
                if (!status)
                {
                    Global.logger.LogMessageEx("Error", "Error updating report after sending statistical report.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception sending monthly statistical reports: {0}", ex.Message);
                return false;
            }
        }
    }
}

