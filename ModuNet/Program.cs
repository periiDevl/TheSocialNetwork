using System;

namespace ModuNet
{
    class MainClass
    {
        public static UserDatabase datab = new UserDatabase();
        public static void Main(string[] args)
        {
            datab.manual();
            datab.create("Database.db");
            datab.register("Itay Natan", "Password", "Email@hi", "872879");
            datab.showUsers();
            datab.who();
            datab.close();
        }
    }
}