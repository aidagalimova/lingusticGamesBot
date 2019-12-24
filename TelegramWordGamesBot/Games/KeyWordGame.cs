using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TelegramWordGamesBot
{
    class KeyWordGame : Game
    {
        private class UserData
        {
            public UserData(State state)
            {
                var arr = state.GameState.Split(';');
                if (arr.Length != 3) throw new ArgumentException("неверный state");
                Points = int.Parse(arr[0]);
                Word = arr[1];
                var temp = arr[2].Split(',');
                UsedWords = temp
                    .Take(temp.Length - 1)
                    .ToList();
            }

            public readonly string Word;
            public List<string> UsedWords { get; set; }
            public int Points { get; set; }
        }

        private const int minNumChar = 10;

        public KeyWordGame(string name, string category)
        {
            Name = name;
            Category = CategoryDAO.GetCategory(category);
            Id = GameDAO.GetGame(Name, Category);
        }

        public override string Answer(int userId, string message)
        {
            var user = UserDAO.GetUser(userId);
            var state = StateDAO.GetState(user);
            if (state.GameState == "")
            {
                var word = FindWord(userId, "");
                state.GameState = "0;" + word + ";" + word + ",";
                StateDAO.UpdateState(state);
                return word + ". Чтобы посмотреть набранные очки и закончить игру напишите result";
            }
            var userData = new UserData(state);
            if (message.ToLower() != "result")
            {
                message = message.ToLower();
                if (IsWordRight(userData, message))
                {
                    var prevPoints = userData.Points++;
                    userData.UsedWords.Add(message);
                    state.GameState = userData.Points +
                        new string(state.GameState.Skip(prevPoints.ToString().Length).ToArray()) + message + ",";
                    StateDAO.UpdateState(state);
                    return "правильно! слов найдено: " + userData.Points;
                }
                else
                    return "не засчитано";
            }
            state.GameState = "";
            StateDAO.UpdateState(state);
            return string.Format("Итог: {0}. Перейти к списку игр /List_of_games", userData.Points);
        }

        private bool IsWordRight(UserData userData, string message)
        {
            if (userData.UsedWords.Contains(message))
                return false;

            if (IsSubword(message, userData.Word.ToLower()))
            {
                var conn = DatabaseConnector.GetConnection();
                string sql = "select word.name from word where( word.name = '" + message + "')";
                NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                conn.Open();
                bool name;
                var result = comm.ExecuteReader();
                name = result.Read();
                conn.Close();
                return name;
            }
            return false;

        }
        private bool IsSubword(string message, string originalWord)
        {
            var builder = new StringBuilder(originalWord);
            foreach (var c in message)
            {
                if (builder.ToString().Contains(c))
                    builder.Remove(builder.ToString().IndexOf(c), 1);
                else
                    return false;
            }
            return true;
        }

        protected override string FindWord(int userId, string pattern)
        {
            return WordDAO.FindRandomWord(Category, minNumChar);
        }
    }
}
