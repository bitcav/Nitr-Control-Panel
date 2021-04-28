using System;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace NitrControlPanel
{
    public struct Inputs
    {
        public string port;
        public bool openBrowser;
        public bool saveLogs;
    }

    public class Config
    {
        private static readonly string rootPath = Directory.GetCurrentDirectory();
        private static readonly string configPath = rootPath + @"\config.ini";


        public static Inputs Read()
        {
            Inputs inputs;

            try
            {
                var reader = new StreamReader(configPath);

                var yaml = new YamlStream();

                yaml.Load(reader);

                var configYML = (YamlMappingNode)yaml.Documents[0].RootNode;

                inputs.port = (string)configYML.Children["port"];
                inputs.openBrowser = Convert.ToBoolean((string)configYML.Children["open_browser_on_startup"]);
                inputs.saveLogs = Convert.ToBoolean((string)configYML.Children["save_logs"]);

                reader.ReadToEnd();
                reader.Close();
                reader.Dispose();

                return inputs;
            }
            catch
            {
                inputs.port = "8000";
                inputs.openBrowser = true;
                inputs.saveLogs = true;
                Write(inputs);
                return inputs;
            }
        }

        public static void Write(Inputs newInputs)
        {
            var input = new StreamReader(configPath);

            var yaml = new YamlStream();
            yaml.Load(input);

            var config = (YamlMappingNode)yaml.Documents[0].RootNode;

            config.Children["port"] = newInputs.port;
            config.Children["open_browser_on_startup"] = newInputs.openBrowser.ToString().ToLower();
            config.Children["save_logs"] = newInputs.saveLogs.ToString().ToLower();


            var output = File.CreateText(configPath + ".tmp");
            yaml.Save(output, assignAnchors: false);
            output.Close();

            input.Close();

            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
            File.Move(configPath + ".tmp", configPath);
        }

        public static void Default()
        {
            string[] lines =
            {
                "port: 8000",
                "open_browser_on_startup: true",
                "save_logs: true"
            };

            File.WriteAllLines("config.ini", lines);

            if (!(File.Exists("nitr.log")))
            {
                File.CreateText("nitr.log");
            }
        }
    }

}
