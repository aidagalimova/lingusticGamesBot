using System.Collections.Generic;

namespace TelegramWordGamesBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // создать разные игры с разными категориями
            var games = new List<Game>
            {
                new CommonWordGame("Игра в слова(Города)", "города мира"),
                new CommonWordGame("Игра в слова(Машины)", "марки автомобилей"),
                new HangManGame("Виселица(Города)", "города мира"),
                new HangManGame("Виселица(Машины)", "марки автомобилей"),
                new KeyWordGame("Слова в слове(Города)", "города мира"),
                new KeyWordGame("Слова в слове(Машины)", "марки автомобилей" )
            };

            CommunicationService.Start(games);


        }
    }
}

