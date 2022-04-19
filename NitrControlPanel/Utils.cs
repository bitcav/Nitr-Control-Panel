using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NitrControlPanel
{
    public class Utils
    {
        public static bool IsAdmin()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }

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

        public static bool InternetOK()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "github.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetLastVersionNumber()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://github.com/bitcav/nitr/releases/latest");
            HttpWebResponse myResp = (HttpWebResponse)req.GetResponse();

            string new_print_url = myResp.ResponseUri.ToString();

            string[] binUrl = new_print_url.Split('/');
            return binUrl[^1];
        }

        public static async Task DownloadAsync(string DownloadLink, string TargetPath, ProgressBar progress, Label labelProgress, Label labelStatus, Label percentLabel, SplashScreen splashScreen)
        {
            await Task.Run(() => DownloadFileWithProgress(DownloadLink, TargetPath, progress, labelProgress, labelStatus, percentLabel));
            splashScreen.Hide();
            labelStatus.Content = "Completed";
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        public static void DownloadFileWithProgress(string DownloadLink, string TargetPath, ProgressBar progress, Label labelProgress, Label labelStatus, Label percentLabel)
        {

            labelStatus.Dispatcher.Invoke(() => labelStatus.Content = "Checking for updates...", DispatcherPriority.Background);

            //Start Download
            // Function will return the number of bytes processed
            // to the caller. Initialize to 0 here.
            int bytesProcessed = 0;

            // Assign values to these objects here so that they can
            // be referenced in the finally block
            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;
            // Use a try/catch/finally block as both the WebRequest and Stream
            // classes throw exceptions upon error

            try
            {
                // Create a request for the specified remote file name
                WebRequest request = WebRequest.Create(DownloadLink);
                if (request != null)
                {
                    // Send the request to the server and retrieve the
                    // WebResponse object 

                    // Get the Full size of the File
                    double TotalBytesToReceive = 0;
                    var SizewebRequest = HttpWebRequest.Create(new Uri(DownloadLink));
                    SizewebRequest.Method = "HEAD";

                    using (var webResponse = SizewebRequest.GetResponse())
                    {
                        var fileSize = webResponse.Headers.Get("Content-Length");
                        TotalBytesToReceive = Convert.ToDouble(fileSize);
                    }

                    response = request.GetResponse();
                    if (response != null)
                    {
                        labelStatus.Dispatcher.Invoke(() => labelStatus.Content = "Updating...", DispatcherPriority.Background);
                        labelProgress.Dispatcher.Invoke(() => labelProgress.Visibility = Visibility.Visible, DispatcherPriority.Background);
                        percentLabel.Dispatcher.Invoke(() => percentLabel.Visibility = Visibility.Visible, DispatcherPriority.Background);

                        // Once the WebResponse object has been retrieved,
                        // get the stream object associated with the response's data
                        remoteStream = response.GetResponseStream();

                        // Create the local file

                        string filePath = TargetPath;


                        localStream = File.Create(filePath);

                        // Allocate a 1k buffer
                        byte[] buffer = new byte[1024];
                        int bytesRead = 0;

                        // Simple do/while loop to read from stream until
                        // no bytes are returned
                        do
                        {

                            // Read data (up to 1k) from the stream
                            bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                            // Write the data to the local file
                            localStream.Write(buffer, 0, bytesRead);

                            // Increment total bytes processed
                            bytesProcessed += bytesRead;


                            double bytesIn = double.Parse(bytesProcessed.ToString());
                            double percentage = bytesIn / TotalBytesToReceive * 100;
                            percentage = Math.Round(percentage, 0);


                            // Safe Update
                            //Increment the progress bar
                            var progressVal = int.Parse(Math.Truncate(percentage).ToString());

                            progress.Dispatcher.Invoke(() => progress.Value = progressVal, DispatcherPriority.Background);



                            //Set the label progress Text
                            var labelVal = int.Parse(Math.Truncate(percentage).ToString()).ToString();

                            labelProgress.Dispatcher.Invoke(() => labelProgress.Content = labelVal, DispatcherPriority.Background);


                        } while (bytesRead > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch any errors
            }
            finally
            {
                // Close the response and streams objects here 
                // to make sure they're closed even if an exception
                // is thrown at some point
                if (response != null) response.Close();
                if (remoteStream != null) remoteStream.Close();
                if (localStream != null) localStream.Close();
            }
        }

    }

}
