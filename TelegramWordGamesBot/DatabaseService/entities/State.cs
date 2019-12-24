using System;

namespace TelegramWordGamesBot
{
    class State
    {
        public string GameState;
        public User User { get; private set; }
        public int GameId;
        public State(string state, User user, int gameId)
        {
            GameState = state;
            User = user;
            GameId= gameId;
        }
    }
}
