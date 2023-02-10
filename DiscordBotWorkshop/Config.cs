using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.Json;

namespace DiscordBotWorkshop
{
    public class Config
    {
        private static readonly string path = "config.json";
        public string Token { get; set; }
        public string Prefix { get; set; }
        public Config()
        {
            Token = "";
            Prefix = "!";
        }
        public Config(string token, string prefix)
        {
            Token = token;
            Prefix = prefix;
        }
        public static Config LoadConfig()
        {
            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<Config>(json);
            }
            catch
            {
                return null;
            }
        }
        public static void SaveConfig(Config config)
        {
            var json = JsonSerializer.Serialize(config);
            File.WriteAllText(path, json);
        }
        public bool HasToken()
        {
            return String.IsNullOrEmpty(Token);
        }
    }
}
