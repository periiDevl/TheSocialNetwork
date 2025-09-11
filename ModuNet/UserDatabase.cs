using System;
using Microsoft.Data.Sqlite;

namespace ModuNet
{
    public class UserDatabase : Module
    {
        private SqliteConnection conn;
        private SqliteCommand cmd;
        public UserDatabase()
        {
            Console.WriteLine("Checking Database...");
        }
        public void create(string filePath)
        {
            conn = new SqliteConnection($"Data Source={filePath}");
            conn.Open();

            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS usersDatabase
(
    id INTEGER PRIMARY KEY,
    username TEXT,
    password TEXT,
    email TEXT,
    phone TEXT
)
            ";
            cmd.ExecuteNonQuery();
        }
        public void register(string username, 
            string password,
            string email,
            string phone)
        {
             cmd.CommandText = @"INSERT INTO usersDatabase (username,password,email,phone) 
             VALUES (@username,@password,@email,@phone)";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.ExecuteNonQuery();
        }
        public void showUsers()
        {
            cmd.CommandText = "SELECT id, username, password, email, phone FROM usersDatabase";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id"]}, Username: {reader["username"]}, Password: {reader["password"]}, Email: {reader["email"]}, Phone: {reader["phone"]}");
                }
            }
        }

        public void close()
        {
            cmd?.Dispose();
            conn?.Close();
            conn?.Dispose();
        }

        public override void manual()
        {
            Console.WriteLine("I am a database");
        }
        public override void who()
        {
            Console.WriteLine("USER-DATABASE");
        }
    }
}
