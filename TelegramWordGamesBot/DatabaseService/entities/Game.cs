using System;

namespace TelegramWordGamesBot
{
    abstract class Game
    {
        public string Name;
        public Category Category;
        public int Id;
        //ответ игры пользователю
        public abstract string Answer(int userId, string message);
        protected void UpdateState(State state)
        {
            StateDAO.UpdateState(state);
        }
        protected abstract string FindWord(int userId, string pattern);
    }
}
