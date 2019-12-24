using System;

namespace TelegramWordGamesBot
{
    class User
    {
        public int ChatId { get;private set; }
        public int Id {get; private set;}
        public User(int chatId, int id)
        {
            ChatId = chatId;
            Id = id;
        }
    }
}
