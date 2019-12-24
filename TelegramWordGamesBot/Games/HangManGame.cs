using System;
using System.Linq;
using System.Text;

namespace TelegramWordGamesBot
{
    class HangManGame : Game
    {
        private class UserData
        {
            public UserData(State state)
            {
                var arr = state.GameState.Split(';');
                if (arr.Length != 3) throw new ArgumentException("неверный state");
                Word = arr[0];
                NowWord = arr[1];
                NumTry = int.Parse(arr[2]);
            }

            public readonly string Word;
            public string NowWord { get; set; }
            public int NumTry { get; set; }
        }

        public HangManGame(string name, string category)
        {
            Name = name;
            Category = CategoryDAO.GetCategory(category);
            Id = GameDAO.GetGame(Name, Category);
        }
        public override string Answer(int userId, string message)
        {
            var user = UserDAO.GetUser(userId);
            var state = StateDAO.GetState(user);
            if (message.Length != 1)
                return "некорректный ввод, введите одну букву.";
            if (state.GameState == "")
            {
                var word = FindWord(userId, "");
                var nowWord = new string(word
                    .Select(c =>
                    {
                        if (char.IsLetter(c))
                            return '*';
                        return c;
                    })
                    .ToArray());
                state.GameState = word + ";" + nowWord + ";0";
                Console.WriteLine(state.GameId);
                Console.WriteLine(state.GameState);
                StateDAO.UpdateState(state);
                return "отгадайте слово: " + nowWord + ". Введите букву.";
            }
            var userData = new UserData(state);
            if (userData.NowWord.Contains('*'))
            {
                var a = Logic(userData, message.ToLower().First());
                userData.NowWord = a.Item1;
                var b = Logic(userData, message.ToUpper().First());
                userData.NowWord = b.Item1;
                if (a.Item2 == false && b.Item2 == false)
                    userData.NumTry++;
                state.GameState = userData.Word + ";" + userData.NowWord + ";" + userData.NumTry;
                StateDAO.UpdateState(state);
            }

            if (userData.NowWord.Contains('*'))
                return string.Format("{0}, использовано попыток: {1}", userData.NowWord, userData.NumTry);
            state.GameState = "";
            StateDAO.UpdateState(state);
            return string.Format("Вы отгадали слово {0}!, использовано попыток: {1}", userData.Word, userData.NumTry);
        }

        private Tuple<string, bool> Logic(UserData userData, char message)
        {
            var h = false;
            var mask = new StringBuilder(userData.NowWord);
            for (int i = userData.Word.IndexOf(message); i > -1; i = userData.Word.IndexOf(message, i + 1))
            {
                mask[i] = message;
                h = true;
            }
            return new Tuple<string, bool>(mask.ToString(), h);
        }

        protected override string FindWord(int userId, string pattern)
        {
            return WordDAO.FindRandomWord(Category);
        }
    }
}