namespace Logs_Core
{
    public class Core
    {
        

        public Core()
        {

        }

        public static void AddLog(string text, DateTime time, string from, int logLevel)
        {
            switch(logLevel)
            {
                case 0: //стадии работы
                    {
                        Console.WriteLine($"{time} >> {from}\n\tstart_log\n\t\t{text}\n\tend_log\n");

                        break;
                    }
                case 1: //апдейты
                    {
                        Console.WriteLine($"{time} >> {from}\n\tstart_log\n\t\t{text}\n\tend_log\n");

                        break;
                    }
                case 2: //ошибки
                    {
                        Console.WriteLine($"{time} >> {from}\n\tstart_log\n\t\t{text}\n\tend_log\n");

                        break;
                    }
                default:
                    {
                        Console.WriteLine("There is no logLevel");
                        Console.WriteLine($"{time} >> {from}\n\tstart_log\n\t\t{text}\n\tend_log\n");

                        break;
                    }
            }

            
        }
    }
}
