using System;

namespace log4net
{
    internal interface ILog
    {
        void Error(Exception v);
        void Error(string v);
        void Warn(string v);
        void Debug(string v);
        void Info(string v);
        void InfoFormat(string Format, params object[] args);
    }

    internal class Log : ILog
    {
        public void Error(Exception v)
        {
            Console.WriteLine(v);
        }

        public void Error(string v)
        {
            Console.WriteLine(v);
        }

        public void Warn(string v)
        {
            Console.WriteLine(v);
        }

        public void Debug(string v)
        {
            Console.WriteLine(v);
        }

        public void Info(string v)
        {
            Console.WriteLine(v);
        }

        public void InfoFormat(string Format, params object[] args)
        {
            Console.WriteLine(Format, args);
        }
    }
}