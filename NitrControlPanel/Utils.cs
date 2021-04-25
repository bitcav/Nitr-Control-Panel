using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace NitrControlPanel
{
    public class Utils
    {
        public static bool IsPortOpen(int port)
        {
            DateTime initialTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddMilliseconds(400);
            TimeSpan difference = endTime - initialTime;

            try
            {
                using var client = new TcpClient();
                var result = client.BeginConnect("127.0.0.1", port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(difference);
                if (!success)
                {
                    return false;
                }
                client.EndConnect(result);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void ShowPanel(string port)
        {
            string url = $"http://localhost:{port}";
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

    }
}
