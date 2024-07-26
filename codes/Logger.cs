using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Text;

namespace eVerification.codes
{
    public class Logger
    {
        public static void ErrorFunctions(StackTrace stackTrace, StackFrame stackFrame, out string fx, out string caller)
        {
            fx = stackFrame.GetMethod().Name;
            caller = stackTrace.GetFrame(1).GetMethod().Name;
        }

        public static void LogError(string msg, string fx = "", string caller = "", string moreDetails = "")
        {
            Log("Error", msg, fx, caller, moreDetails);
        }

        public static void LogWarning(string msg, string fx = "", string caller = "", string moreDetails = "")
        {
            Log("Warning", msg, fx, caller, moreDetails);
        }

        public static void LogInfo(string msg, string fx = "", string caller = "", string moreDetails = "")
        {
            Log("Info", msg, fx, caller, moreDetails);
        }

        private static void Log(string logType, string msg, string fx = "", string caller = "", string moreDetails = "")
        {
            try
            {
                if (msg.Contains("Thread was being aborted."))
                    return;

                var path_ = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Logger.ClearOldLogs(path_, 10);
                if (!Directory.Exists(path_))
                    Directory.CreateDirectory(path_);

                string path = Path.Combine(path_, DateTime.Now.ToString("yyyyMMdd") + ".log");
                string logEntry = $"{DateTime.Now:dd-MMM-yyy HH:mm:ss}: Function: {fx}: Being Called By: {caller}: {logType}: \n{msg}{(moreDetails.Length == 0 ? "" : ": More Details: " + moreDetails)}";

                using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        streamWriter.WriteLine(logEntry);
                }

                Logger.ClearOldLogs(path, 3);
            }
            catch
            {
            }
        }

        private static void ClearOldLogs(string path, int days)
        {
            try
            {
                foreach (FileSystemInfo fileSystemInfo in new DirectoryInfo(path).GetFiles().Where(p => p.LastWriteTime.AddDays(days) < DateTime.Now))
                    File.Delete(fileSystemInfo.FullName);
            }
            catch
            {
            }
        }

        public static string SerializeToXml(object input)
        {
            XmlSerializer ser = new XmlSerializer(input.GetType());
            string result = string.Empty;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, input);
                memStm.Position = 0;
                result = new StreamReader(memStm).ReadToEnd();
            }

            return result;
        }

        public static void ResponseLogs(string _path, string Msg)
        {
            try
            {
                if (!Directory.Exists(_path))
                    Directory.CreateDirectory(_path);

                var Mydate = DateTime.Now.ToString("yyyyMMdd") + ".log";
                var MyError = Path.Combine(_path, Mydate);

                using (var fs = new FileStream(MyError, FileMode.Append))
                {
                    using (var wt = new StreamWriter(fs))
                    {
                        wt.WriteLine(Msg);
                    }
                }
            }
            catch (Exception ex)
            {
                ResponseLogs(_path, "Error in Function LogErrors " + ex);
            }
        }
    }





    /// <summary>
    ///  Extension methods for Exception class.
    /// </summary>
    static public class ExceptionExtensions
    {
        /// <summary>
        ///  Provides full stack trace for the exception that occurred.
        /// </summary>
        /// <param name="exception">Exception object.</param>
        /// <param name="environmentStackTrace">Environment stack trace, for pulling additional stack frames.</param>
        public static string ToLogString(Exception exception, string environmentStackTrace)
        {
            List<string> environmentStackTraceLines = ExceptionExtensions.GetUserStackTraceLines(environmentStackTrace);
            environmentStackTraceLines.RemoveAt(0);

            List<string> stackTraceLines = ExceptionExtensions.GetStackTraceLines(exception.StackTrace);
            stackTraceLines.AddRange(environmentStackTraceLines);

            string fullStackTrace = String.Join(Environment.NewLine, stackTraceLines);

            string logMessage = FlattenException(exception) + Environment.NewLine + fullStackTrace;
            return logMessage;
        }

        /// <summary>
        ///  Gets a list of stack frame lines, as strings.
        /// </summary>
        /// <param name="stackTrace">Stack trace string.</param>
        private static List<string> GetStackTraceLines(string stackTrace)
        {
            return stackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }

        private static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///  Gets a list of stack frame lines, as strings, only including those for which line number is known.
        /// </summary>
        /// <param name="fullStackTrace">Full stack trace, including external code.</param>
        private static List<string> GetUserStackTraceLines(string fullStackTrace)
        {
            List<string> outputList = new List<string>();
            Regex regex = new Regex(@"([^\)]*\)) in (.*):line (\d)*$");

            List<string> stackTraceLines = ExceptionExtensions.GetStackTraceLines(fullStackTrace);
            foreach (string stackTraceLine in stackTraceLines)
            {
                if (!regex.IsMatch(stackTraceLine))
                {
                    continue;
                }

                outputList.Add(stackTraceLine);
            }

            return outputList;
        }
    }
}
