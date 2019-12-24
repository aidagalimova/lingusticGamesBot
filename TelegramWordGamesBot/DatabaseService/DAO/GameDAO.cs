using Npgsql;
using System;

namespace TelegramWordGamesBot
{
    class GameDAO
    {
        private static NpgsqlConnection conn = DatabaseConnector.GetConnection();
        // добавить в таблицу игра новую запись
        // вернуть экземпляр класса для этой записи
        private static int CreateGame(string name, Category category)
        {
            string sql = "insert into game(name, category_id) values('" + name + "','" + category.Id + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open();
            var result = comm.ExecuteReader();
            var id = -1;
            if (result.Read())
            {
                id = Convert.ToInt32(result["id"].ToString());
            }
            conn.Close();
            return id;
        }

        // вернуть экземпляр класса для записи из таблицы категория, если эта запись существует,
        // иначе вызвать метод CreateCategory
        public static int GetGame(string name, Category category)
        {
            string sql = "select * from game where(name='" + name + "' and category_id='" + category.Id + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open();
            var result = comm.ExecuteReader();
            var hasRows = result.HasRows;
            var id = 0;
            if (result.Read())
            {
                id = Convert.ToInt32(result["id"].ToString());
            }
            conn.Close();
            if (!hasRows)
            {
                return CreateGame(name, category);
            }
            return id;
        }
    }
}
