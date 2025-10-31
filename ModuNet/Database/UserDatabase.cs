using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ModuNet
{
    public class UserDatabase : Module
    {
        Dictionary<int, string> users = new Dictionary<int, string>();

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
        public void delete(string name)
        {
            //Validate with hash the password
            cmd.CommandText = $@"
DELETE FROM usersDatabase
WHERE username = '{name}';
";
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
        public bool login(string username, string password)
        {
            cmd.CommandText = @"SELECT COUNT(1) FROM usersDatabase 
                        WHERE username = @username AND password = @password";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            var result = cmd.ExecuteScalar();

            if (Convert.ToInt32(result) > 0)
            {
                Console.WriteLine("Login successful!");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
                return false;
            }
        }

        public void close()
        {
            cmd?.Dispose();
            conn?.Close();
            conn?.Dispose();
        }
        public override async void hndlServer(
            ClientHandler handl,
            PacketHandler hanlerPkt,
            Packet pktSend,
            ServerRequestPacketStream serverRequestPacketStream,
            UInt16 ID
            
            )
        {
            pktSend = hanlerPkt.create(ID, 1, 2, 3, 0);
            if (hanlerPkt.getCleanPayload(ref serverRequestPacketStream.pkt).Equals("register"))
            {

                string message = @"Register in this format: \n USERNAME/PASSWORD/EMAIL/PHONE";
                hanlerPkt.addPayload(message, ref pktSend);
                await handl.sendPacket(pktSend);
                serverRequestPacketStream.pkt = await handl.recivePacket();
                string[] info = serverRequestPacketStream.pkt.payload.Split('/');

                register(info[0], info[1], info[2], info[3]);
                Console.WriteLine($@"Created user with :
USERNAME = {info[0]}
EMAIL = {info[1]}
");
                message = "[+]USER CREATED!";
                hanlerPkt.addPayload(message, ref pktSend);
                await handl.sendPacket(pktSend);

            }
            else if (hanlerPkt.getCleanPayload(ref serverRequestPacketStream.pkt).StartsWith("login"))
            {
                string loginMessage = hanlerPkt.getCleanPayload(ref serverRequestPacketStream.pkt);
                //login <USERNAME><PASSWORD>
                string[] info = loginMessage.Split('<');
                string username = info[1].Split('>')[0];
                string password = info[2].Split('>')[0];
                bool exists = login(username, password);
                string message = "This user does not exist.";
                if (exists)
                {
                    message = "You are logged in as : " + username;
                    users.Add(handl.clientID, username);
                }
                hanlerPkt.addPayload(message, ref pktSend);
                await handl.sendPacket(pktSend);
            }
            else if (hanlerPkt.getCleanPayload(ref serverRequestPacketStream.pkt).StartsWith("?"))
            {
                string message = "";
                message += "YOUR DATA: [\n";
                message += $"ID :[|{handl.clientID}|]]\n";
                string username = "USER";
                if (users.ContainsKey(handl.clientID)) {
                    username = users[handl.clientID];
                }
                message += $"LOGGED AS :[|{username}|]\n";
                message += "\n]";
                hanlerPkt.addPayload(message, ref pktSend);
                await handl.sendPacket(pktSend);
            }
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
