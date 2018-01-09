﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using DSharpPlus.VoiceNext.Codec;
using Newtonsoft.Json;
using SkeletonBot.Exceptions;

namespace SkeletonBot
{
    public static class Program
    {
        private static DiscordClient discord;
        private static CommandsNextExtension commands;
        private static InteractivityExtension interactivity;
        private static VoiceNextExtension voice;

        public static async Task Main(string[] args)
        {
            Globals.BotSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Path.Combine(Globals.AppPath, "config.json")));

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Globals.BotSettings.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
                AutoReconnect = true
            });

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new []{""}, // TODO Add command prefix(es)
                EnableMentionPrefix = true
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            commands.CommandExecuted += Commands_CommandExecuted;
            commands.CommandErrored += Commands_CommandErrored;

            interactivity = discord.UseInteractivity(new InteractivityConfiguration{ });

            voice = discord.UseVoiceNext(new VoiceNextConfiguration
            {
                VoiceApplication = VoiceApplication.Music
            });

            discord.MessageCreated += Discord_MessageCreated;

            await discord.ConnectAsync();

            await Task.Delay(-1);
        }

        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            if (e.Exception is OutputException)
            {
                await e.Context.RespondAsync(e.Exception.Message);
            }
            else
            {
                e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().FullName, e.Exception.ToString(), DateTime.Now);
            }
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            return Task.CompletedTask;
        }

        private static async Task Discord_MessageCreated(MessageCreateEventArgs e)
        {
        }
    }
}
