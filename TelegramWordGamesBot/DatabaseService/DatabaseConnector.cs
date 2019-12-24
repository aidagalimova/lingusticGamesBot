using Npgsql;


namespace TelegramWordGamesBot
{
    class DatabaseConnector
    {
        private static string Server = "127.0.0.1";
        private static string Port = "5432";
        private static string User = "postgres";
        private static string Password = "15171517";
        private static string Database = "telegram_word_games";
        private static NpgsqlConnection connection = null;

        // если подключение к бд есть - вернуть его, иначе подключится
        public static NpgsqlConnection GetConnection()
        {

            if (connection != null)
            {
                return connection;
            }
            else
            {
                string conn_param = "Server=" + Server +
                    ";Port=" + Port +
                    ";User Id =" + User +
                    ";Password=" + Password +
                    ";Database=" + Database;
                connection = new NpgsqlConnection(conn_param);
                return connection;
            }
        }
    }
}
