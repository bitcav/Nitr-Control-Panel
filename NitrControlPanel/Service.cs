using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows;

namespace NitrControlPanel
{
    public class Service
    {
        private static readonly string rootPath = Directory.GetCurrentDirectory();
        private static readonly string ServiceBinPath = rootPath + @"\nitr.exe";
        private static readonly string mainProcess = "nitr";
        private static readonly string serviceName = "NitrService";


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
                string errorMsg = "File nitr.exe does not exist.";
                MessageBox.Show(errorMsg);
                using (StreamWriter sw = File.AppendText(@"nitr.log"))
                {
                    string errorLog = DateTime.Now + " " + errorMsg;
                    sw.WriteLine(errorLog);
                }

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

        public static void Install()
        {
            try
            {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.WorkingDirectory = Path.GetDirectoryName(ServiceBinPath);
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.Arguments = $"/C sc create {serviceName} binPath={ServiceBinPath} DisplayName= \"NITR Service\" start=auto ";
                    cmd.Start();
            
            } catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        public static void Uninstall()
        {
            try
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.Arguments = $"/C sc delete NitrService";
                cmd.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static bool Exists()
        {
            return ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(serviceName));
        }

        public static string StartService()
        {
            ServiceController service = new ServiceController(serviceName);

            if (service == null)
            {
                return "";
            }

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
            var process = Process.GetProcessesByName(mainProcess);
            if (process.Length > 0)
            {
                return process[0].Id.ToString();
            }
            else
            {
                return "- - - - -";
            }
        }

        public static void StopService()
        {
            ServiceController service = new ServiceController(serviceName);
            if (service == null)
            {
                return;
            }

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped);
        }

    }
}
