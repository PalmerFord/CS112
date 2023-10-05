using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fib
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("floor(lg lg n)");
            while (true)
            {
                Console.Write("\nEnter N: ");
                int n = int.Parse(Console.ReadLine());
                int lglgn = LgLgn(n);
                Console.WriteLine("\"floor(lg lg {0}) = {1}.", n, lglgn);
            }
        }
        static int LgLgn(int n)
        {
            return Log(Log(n) - 1) - 1; 
        }
        static int Log(int n)
        {
            if(n/2 >= 1)
            {
                return 1 + Log(n / 2);
            }
            else
            {
                return n;
            }
        }
    }
}