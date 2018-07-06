using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Vocabulary
{
    class Program
    {
        static void Main(string[] args)
        {
            var dict = new DictionaryDB("dictionary.txt");
            string com = "";
            Console.Write("help - помощь\n\n");
            while (true)
            {
                Console.Write("command >> ");
                com = Console.ReadLine();
                if (com == "q" || com == "exit" || com == "quit")
                    break;
                DictionaryMng.CommandAnalyze(dict, com);
            }

            dict.Save("dictionary.txt");
        }
    }
}
