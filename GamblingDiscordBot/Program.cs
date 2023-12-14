using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Data.SQLite;

namespace GamblingDiscordBot
{
    public class Program
    {
        public static Task Main(string[] args) => new Program().MainAsync();

        private DiscordSocketClient _client;

        public SQLiteConnection SQLiteCon;
        public SQLiteCommand SQLiteCmd;

        public Random random = new Random();
        public List<Player> players = new();

        public async Task MainAsync()
        {
            // SQL shit
            SQLiteCon = new SQLiteConnection("Data Source=GamblingBot.db; Version = 3; new = true; Compress = true; ");

            try
            {
                SQLiteCon.Open();
                SQLiteCmd = SQLiteCon.CreateCommand();

                SQLiteCmd.CommandText = "CREATE TABLE IF NOT EXISTS Players(PlayerID INT, Cash INT, Level INT, XP INT, NoOfDice INT, DieHighestNum INT)";
                SQLiteCmd.ExecuteNonQuery();

                Console.WriteLine("Database open");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            SQLiteCmd.CommandText = "SELECT * FROM TblAccounts";
            using (var reader = SQLiteCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    players.Add(new Player
                    (
                            ulong.Parse(reader.GetValue(reader.GetOrdinal("PlayerId")).ToString()),
                            new stats(
                                reader.GetValue(reader.GetOrdinal("Cash")),
                                reader.GetValue(reader.GetOrdinal("Level")),
                                reader.GetValue(reader.GetOrdinal("XP"))
                            ),
                            new upgrades(
                                reader.GetValue(reader.GetOrdinal("NoOfDice")),
                                reader.GetValue(reader.GetOrdinal("DieHighestNum"))
                            )
                    ));
                }
                reader.Close();
            }


            // Discord shit
            _client = new DiscordSocketClient();
            _client.Log += Log;

            var token = File.ReadAllText("../../../../token.txt");
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;
            _client.ButtonExecuted += ButtonHandler;


            // just gotta have it
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task Client_Ready()
        {
            await _client.SetGameAsync("/roll | /help");
            await _client.BulkOverwriteGlobalApplicationCommandsAsync(new ApplicationCommandProperties[0]);

            var testGuild = _client.GetGuild(ulong.Parse("768570410286448663"));
            var buildGuild = _client.GetGuild(ulong.Parse("1038076925106716682"));

            List<SlashCommandBuilder> testCommands = new List<SlashCommandBuilder>();
            List<SlashCommandBuilder> buildCommands = new List<SlashCommandBuilder>();


            // Make the Commands
            buildCommands.Add(
                new SlashCommandBuilder()
                    .WithName("help")
                    .WithDescription("Shows help for the bot")
                );

            testCommands.Add(
                new SlashCommandBuilder()
                    .WithName("roll")
                    .WithDescription("Rolls your die to make money")
                );

            try
            {
                foreach (var command in testCommands)
                {
                    await testGuild.CreateApplicationCommandAsync(command.Build());
                }

                foreach (var command in buildCommands)
                {
                    await testGuild.CreateApplicationCommandAsync(command.Build());
                    await buildGuild.CreateApplicationCommandAsync(command.Build());
                }
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception, Formatting.Indented);
                Console.WriteLine(json);
            }
        }


        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "help":
                    await HandleHelpCommand(command);
                    break;

                case "roll":
                    await HandleRollCommand(command);
                    break;
            }
        }

        private async Task HandleHelpCommand(SocketSlashCommand command)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Command List")
                .WithColor(Color.Purple)
                .AddField("Misc", "`/help` Displays this window\n" +
                                  "`/daily` Get your daily chips")

                .AddField("Shop", "`/shop` View the shop main menu\n" +
                                  "`/upgrades` View the upgrade shop")

                .AddField("Gambling", "`/roll` Roll your dice for a chance to win\n" +
                                      "`/higherlower` Start a game of higher or lower\n");



            await command.RespondAsync(embed: embedBuilder.Build());
        }

        private async Task HandleRollCommand(SocketSlashCommand command)
        {
            var player = getPlayerDetails(command.User.Id);
        }

        private async Task HandleRolesCommand(SocketSlashCommand command)
        {
            var guildUser = (SocketGuildUser)command.User;
            var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(guildUser.Nickname, guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Roles")
                .WithDescription(roleList)
                .WithColor(Color.Purple);

            await command.RespondAsync(embed: embedBuilder.Build());
        }

        private async Task HandleButtonTestCommand(SocketSlashCommand command)
        {
            var componentBuilder = new ComponentBuilder()
                .WithButton("label", "custom-id");

            await command.RespondAsync("Text", components: componentBuilder.Build());
        }


        private async Task ButtonHandler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "custom-id":
                    await component.RespondAsync($"{component.User.Mention} has clicked the button!");
                    break;
            }
        }

        Player getPlayerDetails(ulong playerID)
        {
            Player player;

            player = new Player(0, new stats(), new upgrades());

            return player;
        }
    }
}