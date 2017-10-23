using BolTDLCore.NetStandard;
using Newtonsoft.Json;

namespace BolTDLConsole.NetCore
{
    internal class BolTdlConsoleSettings
    {
        public static readonly string FileName = "BolTDLConsoleSettings.JSON";

        [JsonConstructor]
        public BolTdlConsoleSettings()
        {
        }

        public BolTdlConsoleSettings(bool colors, bool descriptions)
        {
            UseColors = colors;
            UseDescriptions = descriptions;
        }

        public BolTdlConsoleSettings(bool colors, bool descriptions, string usn, string password) : this(colors, descriptions)
        {
            Username = usn;
            Password = password;
        }

        public bool UseColors { get; set; }
        public bool UseDescriptions { get; set; }
        public bool UserWebSync { get; set; }
        public string WebAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public static BolTdlConsoleSettings SettingsFromJson(string json)
        {
            return JsonConvert.DeserializeObject<BolTdlConsoleSettings>(json);
        }

        public void ExportSettings()
        {
            var settings = JsonConvert.SerializeObject(this);
            DataHandler.ExportSettings(FileName, settings);
        }
    }
}