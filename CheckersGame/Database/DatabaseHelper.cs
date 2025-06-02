using CheckersGame.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CheckersGame
{
    public static class DatabaseHelper
    {
        // Строка подключения к базе данных
        private static string connectionString = "Server=LAPTOP-MNNMVMPU\\MSQLSERVER;Database=CheckersGame;Integrated Security=true;";

        // Хеширование пароля
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Регистрация пользователя
        public static bool RegisterUser(string username, string password, string email)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("RegisterUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@PasswordHash", HashPassword(password));
                        command.Parameters.AddWithValue("@Email", email);

                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка регистрации: {ex.Message}");
                return false;
            }
        }

        // Аутентификация пользователя - исправленная версия
        public static User AuthenticateUser(string username, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("AuthenticateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@PasswordHash", HashPassword(password));

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    UserID = (int)reader["UserID"],
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка аутентификации: {ex.Message}");
            }
            return null;
        }

        // Дополнительный метод для совместимости с LoginForm
        public static bool AuthenticateUser(string username, string password, out int userID, out string email)
        {
            userID = 0;
            email = "";

            User user = AuthenticateUser(username, password);
            if (user != null)
            {
                userID = user.UserID;
                email = user.Email;
                return true;
            }
            return false;
        }

        // Получение статистики пользователя
        public static UserStats GetUserStats(int userID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetUserStats", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", userID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserStats
                                {
                                    UserID = userID,
                                    Username = reader["Username"].ToString(),
                                    TotalGames = (int)reader["TotalGames"],
                                    WonGames = (int)reader["WonGames"],
                                    LostGames = (int)reader["LostGames"],
                                    DrawnGames = (int)reader["DrawnGames"],
                                    WinRate = (decimal)reader["WinRate"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка получения статистики: {ex.Message}");
            }
            return null;
        }

        // Получение лучшего игрока
        public static UserStats GetTopPlayer()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetTopPlayer", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Criteria", "WinRate"); // Изменено с "WonGames" на "WinRate"

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserStats
                                {
                                    Username = reader["Username"].ToString(),
                                    TotalGames = (int)reader["TotalGames"],
                                    WonGames = (int)reader["WonGames"],
                                    LostGames = reader["LostGames"] != DBNull.Value ? (int)reader["LostGames"] : 0,
                                    DrawnGames = reader["DrawnGames"] != DBNull.Value ? (int)reader["DrawnGames"] : 0,
                                    WinRate = (decimal)reader["WinRate"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка получения топ игрока: {ex.Message}");
            }
            return null;
        }

        // Создание новой игры
        public static int CreateGame(int player1ID, int? player2ID, int difficultyLevel, string gameMode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Games (Player1ID, Player2ID, DifficultyLevel, GameMode) 
                                   VALUES (@Player1ID, @Player2ID, @DifficultyLevel, @GameMode);
                                   SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Player1ID", player1ID);
                        command.Parameters.AddWithValue("@Player2ID", (object)player2ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DifficultyLevel", difficultyLevel);
                        command.Parameters.AddWithValue("@GameMode", gameMode);

                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка создания игры: {ex.Message}");
                return -1;
            }
        }

        // Завершение игры
        public static void FinishGame(int gameID, int? winnerID, string gameResult)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Games 
                                   SET EndTime = GETDATE(), GameStatus = 'Finished', WinnerID = @WinnerID, GameResult = @GameResult
                                   WHERE GameID = @GameID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@GameID", gameID);
                        command.Parameters.AddWithValue("@WinnerID", (object)winnerID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@GameResult", gameResult);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка завершения игры: {ex.Message}");
            }
        }

        // Обновление статистики после игры
        public static void UpdateGameStats(int userID, string gameResult)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UpdateGameStats", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@GameResult", gameResult);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка обновления статистики: {ex.Message}");
            }
        }
    }
}