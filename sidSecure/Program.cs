using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.DirectoryServices;
using MySql.Data.MySqlClient;

namespace sidSecure
{
    class Program
    {
        public static SecurityIdentifier GetSID()
        {
            return new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid;
        }
        static void Main(string[] args)
        {
            Console.Title = "sidSecure - SID Secure System by goldblack";
            string SID = GetSID().ToString();
            new MySQLConnect(SID);
        }
    }

    class MySQLConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public MySQLConnect(string SID)
        {
            Start(SID);
        }

        private void Start(string SID)
        {
            server = "localhost"; // CHANGE ME
            database = "sid";
            uid = "root"; // CHANGE ME
            password = ""; // CHANGEME
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);

            Select(SID);
        }
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR: Cannot connect to server.");
                        break;

                    case 1045:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR: Unknown error.");
                        break;
                }
                return false;
            }
        }
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public List<string>[] Select(string SID)
        {
            string query = "SELECT * FROM sidsecure";

            List<string>[] list = new List<string>[3];
            list[0] = new List<string>();
            list[1] = new List<string>();

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    list[0].Add(dataReader["name"] + "");
                    list[1].Add(dataReader["sid"] + "");
                }

                if (list[1].Contains(SID))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Successfully authenticated. SID: (" + SID + ")");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Authentication failure. SID: (" + SID + ")");
                }
                Console.ReadKey(true);
                dataReader.Close();
                this.CloseConnection();
                return list;
            }
            else
            {
                return list;
            }
        }
    }
}