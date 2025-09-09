using System;
namespace ModuNet
{
    public class Module
    {
        public Module()
        {
        }
        public virtual void manual()
        {
            string man = "Basic manual,\n Hello World! ";
            Console.WriteLine(man);
        }
        public virtual void who()
        {
            Console.WriteLine("Empty Module");
        }
    }
}
