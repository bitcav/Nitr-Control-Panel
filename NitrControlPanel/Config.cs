using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace NitrControlPanel
{
    public struct Inputs
    {
        public string port;
        public bool openBrowser;
        public bool saveLogs;
    }
    class Config
    {
        private readonly string path;
        Inputs inputs;

        public Config(string configPath)
        {
            path = configPath;
        }

        public Inputs Read()
        {
            try
            {
                var reader = new StreamReader(path);

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

        public void Write(Inputs newInputs)
        {
            var input = new StreamReader(path);

            var yaml = new YamlStream();
            yaml.Load(input);

            var config = (YamlMappingNode)yaml.Documents[0].RootNode;

            config.Children["port"] = newInputs.port;
            config.Children["open_browser_on_startup"] = newInputs.openBrowser.ToString().ToLower();
            config.Children["save_logs"] = newInputs.saveLogs.ToString().ToLower();


            var output = File.CreateText(path + ".tmp");
            yaml.Save(output, assignAnchors: false);
            output.Close();

            input.Close();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Move(path + ".tmp", path);
        }
    }

}
