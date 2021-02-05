using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Timers.Timer timer = new System.Timers.Timer(40000);
            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(DoEvent);
            timer.Start();
            AutomationService.Dotask();
            var input = Console.ReadLine();
            while(input != "end")
            {
                input = Console.ReadLine();
            }
        }
        private static void DoEvent(object sender, ElapsedEventArgs e)
        {
            AutomationService.Dotask();
        }
    }
}
