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
            Console.Title = "Vocabulary trainer";
            var dict = new DictionaryDB("dictionary.txt");
            string com = "";
            DictionaryMng.d = dict;
            Console.Write("help - помощь\n\n");
            while (true)
            {
                Console.Write("command >> ");
                com = Console.ReadLine();
                if (com == "q" || com == "exit" || com == "quit")
                    break;
                DictionaryMng.CommandAnalyze(com);
            }

            dict.Save("dictionary.txt");
        }
    }
}
