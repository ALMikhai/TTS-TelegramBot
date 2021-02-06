using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramTTSBot
{
    class Program
    {
        private static ITelegramBotClient _botClient;
        private static Dictionary<long, bool> _chatsId;

        static void Main()
        {
            _chatsId = new Dictionary<long, bool>();
            _botClient = new TelegramBotClient("Your bot key");

            var me = _botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            _botClient.OnMessage += BotOnMessage;
            _botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            _botClient.StopReceiving();
        }

        static async void BotOnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                var chatId = e.Message.Chat.Id;
                var text = e.Message.Text;
                Console.WriteLine($"Received a text message in chat {chatId} with text {text}.");

                if (!_chatsId.ContainsKey(chatId))
                {
                    _chatsId.Add(chatId, false);
                }
                switch (text)
                {
                    case "/start":
                        var rkm = new ReplyKeyboardMarkup();
                        rkm.Keyboard =
                            new KeyboardButton[][]
                            {
                                new KeyboardButton[] {"/say", "/break"},
                            };
                        await _botClient.SendTextMessageAsync(chatId, "Choose the action", replyMarkup: rkm);
                        break;
                    case "/say":
                        _chatsId[chatId] = true;
                        await _botClient.SendTextMessageAsync(chatId, "Print the message for transformation to speech");
                        break;
                    case "/break":
                        _chatsId[chatId] = false;
                        await _botClient.SendTextMessageAsync(chatId, "Your next message will not be converted");
                        break;
                    default:
                        if (_chatsId[chatId] == true)
                        {
                            var responsePath = $"Responses/{chatId}.wav";
                            var error = PythonTTS.TextToSpeech(text, responsePath);
                            if (string.IsNullOrWhiteSpace(error))
                            {
                                Console.WriteLine($"Successfully transformed message {text}, if {chatId}");
                                await using var stream = System.IO.File.OpenRead(responsePath);
                                await _botClient.SendVoiceAsync(
                                    chatId: chatId,
                                    voice: stream
                                );
                            }
                            else
                            {
                                Console.WriteLine($"Error in transformation message {text} to speech, id {chatId}");
                                Console.WriteLine(error);
                            }
                        }
                        break;
                }
            }
        }
    }
}
