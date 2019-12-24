using System;
using System.Collections.Generic;
using System.Linq;

namespace TelegramWordGamesBot
{
    class CommonWordGame : Game
    {

        public CommonWordGame(string Name, string Category)
        {
            this.Name = Name;
            this.Category = CategoryDAO.GetCategory(Category);
            Id = GameDAO.GetGame(Name, this.Category);
        }

        private char GetLastUsedChar(int userId)
        {
            var state = StateDAO.GetState(UserDAO.GetUser(userId));
            if (state.GameState.Length == 0)
            {
                return ' ';
            }
            else
            {
                return Convert.ToChar(state.GameState.Split('@')[0]);
            }
        }
        private List<string> GetUsedWords(int userId)
        {
            var state = StateDAO.GetState(UserDAO.GetUser(userId));
            if (state.GameState.Length == 0)
            {
                return new List<string>();
            }
            else
            {
                return state.GameState.Split('@')[1].Split(',').ToList();
            }
        }

        private State ConvertToState(char LastUsedChar, List<string> UsedWords, int userId)
        {
            var state = StateDAO.GetState(UserDAO.GetUser(userId));
            state.GameState = LastUsedChar + "@" + string.Join(",", UsedWords.ToArray());
            return state;
        }

        public override string Answer(int userId, string message)
        {
            var UsedWords = GetUsedWords(userId);

            if (!IsWordRight(userId, message) && UsedWords.Count != 0)
            {
                return "Слово не подходит";
            }
            else if (IsWordUsed(userId, message))
            {
                return "Слово уже было использовано";
            }
            else
            {
                return Logic(userId, message);
            };
        }

        private string Logic(int userId, string text)
        {
            var LastUsedChar = GetLastUsedChar(userId);
            var UsedWords = GetUsedWords(userId);
            char userChar;
            if (text[text.Length - 1] == 'ь')
            {
                userChar = text.ToLower()[text.Length - 2];
            }
            else
            {
                userChar = text.ToLower()[text.Length - 1];
            }
            Console.WriteLine(userChar);
            var word = FindWord(userId, userChar.ToString());
            UsedWords.Add(text.ToLower());
            if (word != "не нашлось")
            {
                if (word[word.Length - 1] == 'ь')
                {
                    LastUsedChar = word[word.Length - 2];
                }
                else
                {
                    LastUsedChar = word[word.Length - 1];
                }
            }
            Console.WriteLine(text);
            UsedWords.Add(word);
            UpdateState(ConvertToState(LastUsedChar, UsedWords, userId));
            return word;
        }

        private bool IsWordRight(int userId, string text)
        {
            var LastUsedChar = GetLastUsedChar(userId);
            return LastUsedChar == text.ToLower()[0] && WordDAO.IsCorrectWord(text, Category.Name);
        }

        private bool IsWordUsed(int userId, string text)
        {
            var UsedWords = GetUsedWords(userId);
            return UsedWords.Contains(text.ToLower());
        }

        protected override string FindWord(int userId, string pattern)
        {
            var UsedWords = GetUsedWords(userId);
            var word = "не нашлось";
            var words = WordDAO.FindWordsStartWith(pattern, Category.Name).GetEnumerator();
            while (words.MoveNext())
            {
                if (!UsedWords.Contains(words.Current))
                {
                    word = words.Current;
                    break;
                }
            }
            return word;
        }
    }
}