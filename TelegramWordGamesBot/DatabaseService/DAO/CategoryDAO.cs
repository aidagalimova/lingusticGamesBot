using Npgsql;
using System;

namespace TelegramWordGamesBot
{
    class CategoryDAO
    {
        private static NpgsqlConnection conn = DatabaseConnector.GetConnection();
        // добавить в таблицу категория новую запись
        // вернуть экземпляр класса для этой записи
        private static Category CreateCategory(string name)
        {
            string sql = "insert into category(name) values('" + name + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open();
            var result = comm.ExecuteReader();
            conn.Close();
            return new Category(name, Convert.ToInt32(result["id"].ToString()));
        }

        // вернуть экземпляр класса для записи из таблицы категория, если эта запись существует,
        // иначе вызвать метод CreateCategory
        public static Category GetCategory(string name)
        {
            string sql = "select * from category where(name='" + name + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open();
            var result = comm.ExecuteReader();
            var hasRows = result.HasRows;
            int id = 0;
            if (result.Read())
            {
                id = Convert.ToInt32(result["id"].ToString());
            }
            conn.Close();
            if (!hasRows)
            {
                return CreateCategory(name);
            }
            return new Category(name, id);
        }
    }
}
