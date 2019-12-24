using Npgsql;
using System;

namespace TelegramWordGamesBot
{
    class UserDAO
    {
        private static NpgsqlConnection conn =  DatabaseConnector.GetConnection();
        // Добавить запись в таблицу пользователи
        private static User CreateUser(int chatId)
        {
            string sql = "insert into users(chat_id) values('" + chatId + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open();
            var result = comm.ExecuteReader();
            var id = -1;
            if(result.Read())
            {
               id = Convert.ToInt32(result["id"].ToString());
            }
            conn.Close();
            return new User(chatId, id);
        }
        
        //Взять из таблицы значение, если эта запись существует, иначе 
        //создать запись
        public static User GetUser(int chatId)
        {  
            string sql = "select * from users where (chat_id ='" + chatId + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            if(conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            var result = comm.ExecuteReader();
            var hasRows = result.HasRows;
            var id = -1;
            if (result.Read())
            {
                id = Convert.ToInt32(result["id"].ToString());
            }
            conn.Close();
            if(!hasRows)
            {
                return CreateUser(chatId);
            }
            return new User(chatId, id);
        }
    }
}
