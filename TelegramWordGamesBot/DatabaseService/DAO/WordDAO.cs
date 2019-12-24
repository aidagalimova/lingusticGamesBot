using Npgsql;
using System;
using System.Collections.Generic;

namespace TelegramWordGamesBot
{
    class WordDAO
    {
        private static NpgsqlConnection conn = DatabaseConnector.GetConnection();
        // проверить есть ли слово в таблице
        public static bool IsCorrectWord(string text, string name)
        {
            conn.Open();
            string sql = "";
            sql = "select word.name from word, word_category,category where(word.name = '" + text + "' " +
                    "AND word.id = word_category.word_id " +
                    "and word_category.category_id = category.id " +
                    "and category.name = '" + name + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            var result = comm.ExecuteReader();
            var hasRows = result.HasRows;
            conn.Close();
            return hasRows;
        }

        // найти слова начинающиеся на определенную букву
        public static IEnumerable<string> FindWordsStartWith(string pattern, string name)
        {
            conn.Open();
            string sql = "";
            sql = "select word.name from word, word_category,category where(word.name ilike '" + pattern + "%' " +
                    "AND word.id = word_category.word_id " +
                    "and word_category.category_id = category.id " +
                    "and category.name = '" + name + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            var result = comm.ExecuteReader();
            while (result.Read())
            {
                yield return result["name"].ToString();
            }
            conn.Close();
        }

        // найти случайное слово из диапазона слов, определенной категории
        public static string FindRandomWord(Category category, int minNumChar = 1)
        {
            var conn = DatabaseConnector.GetConnection();
            var random = new Random();
            var name = "";
            var minInd = 0;
            var maxInd = int.MaxValue -1;
            string sqlmin = "select word.id from word, word_category,category where(word.id = word_category.word_id " +
                             "and word_category.category_id = category.id " +
                             "and category.name = '" + category.Name + "') " +
                             "order by word.id limit 1";
            string sqlmax = "select word.id from word, word_category,category where(word.id = word_category.word_id " +
                             "and word_category.category_id = category.id " +
                             "and category.name = '" + category.Name + "') " +
                             "order by word.id desc limit 1";
            NpgsqlCommand commin = new NpgsqlCommand(sqlmin, conn);
            conn.Open();
            var resultmin = commin.ExecuteReader();
            if (resultmin.Read())
            {
                var min = resultmin["id"].ToString();
                minInd = int.Parse(min);
            }
            conn.Close();

            NpgsqlCommand commax = new NpgsqlCommand(sqlmax, conn);
            conn.Open();
            var resultmax = commax.ExecuteReader();
            if (resultmax.Read())
                maxInd = int.Parse(resultmax["id"].ToString());
            conn.Close();
            while (name == "")
            {
                int randomNum = random.Next(minInd, maxInd + 1);
                Console.WriteLine(randomNum);
                string sql = "select word.name from word, word_category,category where( word.id >= " + randomNum.ToString() +
                        "and length(word.name) >= " + minNumChar.ToString() + ")" +
                        "limit 1";
                NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                conn.Open();
                var result = comm.ExecuteReader();

                if (result.Read())
                    name = result["name"].ToString();
                conn.Close();
            }
            return name;
        }
    }
}
