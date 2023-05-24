using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using Donkey_Kong;

namespace Donkey_Kong
{
    //Constructor
    //Full credit to ChatGPT
    public class HighScore
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }

        public HighScore(string playerName, int score, string formattedDate)
        {
            PlayerName = playerName;
            Score = score;
            Date = DateTime.ParseExact(formattedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }
    }
    //Database helper
    //Deels credit van: ChatGPT & stackoverflow
    public class DatabaseHelper
    {
        public string connectionString;
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.accdb");
        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<HighScore> GetTopHighScores(int limit)
        {
            var highScores = new List<HighScore>();

            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();
                var command = new OleDbCommand($"SELECT TOP {limit} PlayerName, Score, [Date] FROM HighScores ORDER BY Score DESC", con);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string playerName = reader.GetString(0);
                    int score = reader.GetInt32(1);
                    DateTime formattedDate = reader.GetDateTime(2);
                    string formattedDateString = formattedDate.ToString("dd-MM-yyyy");
                    HighScore highScore = new HighScore(playerName, score, formattedDateString);
                    highScores.Add(highScore);
                }
            }

            return highScores;
        }

        //Injection-proof gemaakt met behulp van ChatGPT
        public static void AddOrUpdateHighScore(string name, int score, string formattedDate)
        {
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{Application.StartupPath}\\Database.accdb';Persist Security Info=False;";
            DatabaseHelper dbHelper = new DatabaseHelper(connectionString);

            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();

                var command = new OleDbCommand();
                command.Connection = con;

                // Check if the player already has a high score
                command.CommandText = $"SELECT COUNT(*) FROM HighScores WHERE PlayerName='{name}'";
                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    // Player already has a high score, update it if the new score is higher
                    command.CommandText = $"SELECT Score FROM HighScores WHERE PlayerName='{name}'";
                    int existingScore = (int)command.ExecuteScalar();

                    if (score > existingScore)
                    {
                        command.CommandText = $"UPDATE HighScores SET Score={score}, [Date]='{formattedDate}' WHERE PlayerName='{name}'";
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Player doesn't have a high score yet, create a new entry

                    command.CommandText = "INSERT INTO HighScores (PlayerName, Score, [Date]) VALUES (?, ?, ?)";
                    command.Parameters.AddWithValue("PlayerName", name);
                    command.Parameters.AddWithValue("Score", score);
                    command.Parameters.AddWithValue("[Date]", formattedDate);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
