using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DiscordBotWorkshop.Commands;
using DiscordBotWorkshop.Database;
using Microsoft.Extensions.Logging;

namespace DiscordBotWorkshop
{
    public class Bot
    {
        public string Version { get; private set; }
        public Config Config { get; private set; }
        public static string Prefix { get; private set; }
        public static InteractivityExtension Interactivity { get; private set; }
        public static DiscordClient Client { get; private set; }
        public static UserDatabase UserDatabase { get; private set; }
        public static void Main(string[] Cowbot)
        {
            new Bot().Run();
        }
        public Bot()
        {
            Version = "1.0.0";
        }
        private void Run()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
            Console.WriteLine("GameTime Console");
            SetUp();
        }
        private void SetUp()
        {
            Console.WriteLine("Loading Config");
            Config = Config.LoadConfig();
            if (Config == null)
            {
                Console.WriteLine("First time setup required.");
                //Token and prefix setup
                var token = "";
                var prefix = "";
                while (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Input Bot Token");
                    token = Console.ReadLine();
                }
                while (string.IsNullOrEmpty(prefix))
                {
                    Console.WriteLine("Input Prefix");
                    prefix = Console.ReadLine();
                }
                Config = new Config(token, prefix);
                Prefix = prefix;
                Config.SaveConfig(Config);
                Console.WriteLine("Setup completed.");
            }
            else
            {
                Prefix = Config.Prefix;
                Console.WriteLine("Config Loaded.");
            }

            //Database
            Console.WriteLine("Loading user database");
            var data = UserDatabase.LoadUsers();
            if(data == null)
            {
                Console.WriteLine("No database found. Generating new database...");
                UserDatabase = new UserDatabase();
                UserDatabase.SaveUsers(); //Create file
                Console.WriteLine("Done.");
            }
            else
            {
                UserDatabase = new UserDatabase(data);
                Console.WriteLine("Database loaded.");
            }

            DiscordAsync().GetAwaiter().GetResult();
        }
        private async Task DiscordAsync()
        {
            var client = new DiscordClient(new DiscordConfiguration()
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.MessageContents | DiscordIntents.GuildMessages
            });
            Client = client;
            Interactivity = client.UseInteractivity(new InteractivityConfiguration
            {
                //Default use response timeout
                Timeout = TimeSpan.FromMinutes(5)
            });
            var commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] { Config.Prefix },
                //Test toggle
                IgnoreExtraArguments = true,
                //Maybe turn to false?
                CaseSensitive = false,

                //If time disable and create custom help command
                EnableDefaultHelp = true,
                
            });
            client.Ready += Events.Client_Ready;
            client.ComponentInteractionCreated += Events.AcknowledgeComponentInteraction;
            commands.CommandErrored += Events.CommandErrored;
            commands.RegisterCommands<BlackjackCommands>();
            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}