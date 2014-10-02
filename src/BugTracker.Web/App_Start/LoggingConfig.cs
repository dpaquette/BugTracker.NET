using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace btnet.App_Start
{
    public static class LoggingConfig
    {
        public static void Configure()
        {
            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget();

            fileTarget.FileName = Path.Combine(Util.get_log_folder(), "btnet_log.txt");
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
            fileTarget.ArchiveEvery = FileArchivePeriod.Day;
            config.AddTarget("File", fileTarget);

            var mailTarget = new MailTarget
            {
                UseSystemNetMailSettings = true,
                To = Util.get_setting("ErrorEmailTo", ""),
                From = Util.get_setting("ErrorEmailFrom", ""),
                Subject = "BTNET Error Notification",
                Layout = "${machinename}${newline} ${date} ${newline} ${newline} ${message} ${newline}  ${exception} ${newline}"
            };
            config.AddTarget("Mail", mailTarget);

            //Turn logging on/off based on the LogEnabled setting
            var logLevel = Util.get_setting("LogEnabled", "1") == "1" ? LogLevel.Trace: LogLevel.Off;
            config.LoggingRules.Add(new LoggingRule("*", logLevel, fileTarget));

            var emailLogLevel = Util.get_setting("ErrorEmailEnabled", "1") == "1" ? LogLevel.Fatal : LogLevel.Off;
            config.LoggingRules.Add(new LoggingRule("*", emailLogLevel, mailTarget));

            LogManager.Configuration = config;
        }
    }
}