using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAL.Models
{
    public class ApplicationSettingsHandler
    {
        private static ApplicationSettings staticSettings;//exsiting data
        public readonly ApplicationSettings settings;
        public ApplicationSettingsHandler()
        {
            string path = "appsettings.json";
            string text = File.ReadAllText(path);

            //prevent reading the file multiple times
            if (staticSettings == null)
            {
                settings = JsonConvert.DeserializeObject<ApplicationSettings>(text);
                staticSettings = settings;
            }//converted data already exists
            else
            {
                settings = staticSettings;
            }

        }
    }
    public class ApplicationSettings
    {
        public ApplicationSettingsLogging Logging { get; set; }
        public string? AllowedHosts { get; set; }
        public Dictionary<string, string> methods { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public Dictionary<string, string> Params { get; set; }
        public Dictionary<string, string> firebase { get; set; }



        public class ApplicationSettingsLogging
        {
            public ApplicationSettingsLogLevel LogLevel { get; set; }
        }

        public class ApplicationSettingsLogLevel
        {
            public string? Default { get; set; }
            public string? Microsoft { get; set; }
        }

    }
}
