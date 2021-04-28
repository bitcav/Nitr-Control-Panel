using System.Diagnostics;
using System.IO;

namespace NitrControlPanel
{
    public class Service
    {
        private static readonly string rootPath = Directory.GetCurrentDirectory();
        private static readonly string ServiceBinPath = rootPath + @"\nitr.exe";
        private static readonly string mainProcess = "nitr";

        public static string Start()
        {
            try
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = ServiceBinPath;
                cmd.StartInfo.WorkingDirectory = Path.GetDirectoryName(ServiceBinPath);
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                return cmd.Id.ToString();
            }
            catch
            {
                return "";
            }

        }

        public static void Stop()
        {
            foreach (var process in Process.GetProcessesByName(mainProcess))
            {
                process.Kill();
            }
        }

        public static bool IsRunning()
        {
            Process[] pname = Process.GetProcessesByName(mainProcess);
            if (pname.Length == 0)
                return false;
            else
                return true;
        }

        public static string GetProcessID()
        {
            var process = Process.GetProcessesByName(mainProcess);
            if (process.Length > 0)
            {
                return process[0].Id.ToString();
            } else
            {
                return "- - - - -";
            }

        }
    }
}
