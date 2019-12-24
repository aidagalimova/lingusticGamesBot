using Npgsql;
using System;

namespace TelegramWordGamesBot
{
    class StateDAO
    {
        private static NpgsqlConnection conn = DatabaseConnector.GetConnection();
        // добавить запись в таблицу состояние
        // вернуть экземпляр класса для этой записи
        private static State CreateState(string gameState, User user, int gameId)
        {
            string sql = "insert into state(state,user_id,game_id) values('" + gameState + "','" + user.Id + "','" + gameId + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open();
            var result = comm.ExecuteReader();
            conn.Close();
            return new State(gameState, user, gameId);
        }

        // вернуть экземпляр класса для записи из таблицы состояние, если запись существует
        // иначе создать новое состояние
        public static State GetState(User user, int gameId)
        {
            string sql = "select * from state where(user_id='" + user.Id + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open();
            var result = comm.ExecuteReader();
            var state = "";
            bool hasRows = result.HasRows;
            if (result.Read())
            {
                state = result["state"].ToString();
            }
            conn.Close();
            if (!hasRows)
            {
                return CreateState("", user, gameId);
            }
            return new State(state, user, gameId);
        }

        // вернуть экземпляр класса для записи из таблицы состояние, если запись существует
        // иначе создать пустое состояние с индексом несуществующей игры
        public static State GetState(User user)
        {
            string sql = "select * from state where(user_id='" + user.Id + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open();
            var result = comm.ExecuteReader();
            var hasRows = result.HasRows;
            var state = "";
            var gameId = 3;
            if (result.Read())
            {
                state = result["state"].ToString();
                gameId = Convert.ToInt32(result["game_id"].ToString());
            }
            conn.Close();
            if (!hasRows)
            {
                return CreateState("", user, 3);
            }
            return new State(state, user, gameId);
        }

        // обновить состояние игры пользователя
        public static void UpdateState(State state)
        {
            string sql = "update state SET state = '" + state.GameState + "', game_id = '" + state.GameId + "' where(user_id='" + state.User.Id + "')";
            NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
            conn.Open();
            var result = comm.ExecuteReader();
            conn.Close();
        }
    }
}
