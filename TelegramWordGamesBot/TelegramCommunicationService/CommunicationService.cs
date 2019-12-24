using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace TelegramWordGamesBot
{
    class CommunicationService
    {
        public static TelegramBotClient Bot;
        public static List<Game> Games;
        public static void Start(List<Game> ListOfGames)
        {
            Games = ListOfGames;
            var wp = new HttpToSocks5Proxy("51.158.186.141", 1080);
            wp.ResolveHostnamesLocally = true;
            Bot = new TelegramBotClient("1005798825:AAHH1l3hhyYxlcaUwMPsTFWqdesiqgF-dCk", wp);
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
            Console.WriteLine("Bot started");
            Console.ReadLine();
        }

        // если пользователь выбрал игру, изменить состояние пользователя
        private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var message = e.CallbackQuery;
            foreach (var game in Games)
            {
                if (game.Name == e.CallbackQuery.Data)
                {
                    var state = StateDAO.GetState(UserDAO.GetUser(message.From.Id), game.Id);
                    StateDAO.UpdateState(state);
                    await Bot.SendTextMessageAsync(message.From.Id, "Введите что-нибудь чтобы начать" + game.Name +
                            "! Чтобы завершить игру напишите - exit.");
                    break;
                }
            }
        }
        //если пользователь ввел exit очистить состояние пользователя
        //если пользователь в игре, вызывать метод Answer для нужной игры
        //если пользователь не в игре, выполнять команды
        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null || message.Type != MessageType.Text)
                return;
            Console.WriteLine(message.Text);
            var state = StateDAO.GetState(UserDAO.GetUser(message.From.Id));
            if (message.Text.ToLower() == "exit")
            {
                state.GameState = "";
                state.GameId = 3; //Несуществующая игра
                StateDAO.UpdateState(state);
                await Bot.SendTextMessageAsync(message.From.Id, "Игра завершена! Перейти к списку игр /List_of_games");
            }
            else if (state.GameId != 3) //ID несуществующая игра
            {
                await Bot.SendTextMessageAsync(message.From.Id, GetGame(state.GameId).Answer(message.From.Id, message.Text));
            }
            else
            {
                await MakeCommands(message);
            }
        }

        private static async Task MakeCommands(Message message)
        {
            Console.WriteLine("Make");
            switch (message.Text)
            {
                case "/start":
                    await Bot.SendTextMessageAsync(message.From.Id, "Привет!\n Перейти к списку игр /List_of_games");
                    break;
                case "/List_of_games":
                    var buttons = new InlineKeyboardButton[Games.Count];
                    for (int i = 0; i < Games.Count; i++)
                    {
                        buttons[i] = InlineKeyboardButton.WithCallbackData(Games[i].Name);
                    }

                    for (int i = 0; i < Games.Count; i++)
                    {
                        buttons[i] = InlineKeyboardButton.WithCallbackData(Games[i].Name);
                    }

                    var inlinekeyboard = new InlineKeyboardMarkup(new[]
                    {
                        buttons
                    }
                    );
                    await Bot.SendTextMessageAsync(message.From.Id, "Выберите игру", replyMarkup: inlinekeyboard);
                    break;
            }
        }

        private static Game GetGame(int gameId)
        {
            foreach (var game in Games)
            {
                if (game.Id == gameId)
                {
                    return game;
                }
            }
            throw new ArgumentException("Игры с данным id не существует");
        }

    }
}

