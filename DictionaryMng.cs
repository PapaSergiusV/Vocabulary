using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Vocabulary
{
    class DictionaryMng
    {
        public static void CommandAnalyze(DictionaryDB d, string com)
        {
            if (com == "learn")
                Learning(d);
            else if (com == "help")
            {
                Console.Clear();
                Console.WriteLine("HELP:\nadd\t\tдобавить слова\nremove ...\tудалить слово\nedit\t\tизменить слово\nq / exit\tвыход\n");
            }
            else if (com == "add")
                Add(d);
            else if (Regex.IsMatch(com, @"^remove\s+\w+"))
                d.Remove(Regex.Match(com, @"^remove\s+(.+)$").Groups[1].Value);
            else if (Regex.IsMatch(com, @"^edit\s+\w+"))
            {
                Console.Clear();
                d.Edit(Regex.Match(com, @"^edit\s+(.+)$").Groups[1].Value);
            }
        }

        private static void Add(DictionaryDB d)
        {
            Console.Clear();
            Console.WriteLine("Введите 'q' для выхода\n----------------------");
            while (true)
            {
                Console.Write("Введите англ. слово: ");
                string eng = Console.ReadLine();
                if (eng == "q")
                    break;
                Console.Write("Введите рус. слово:  ");
                string rus = Console.ReadLine();
                if (rus == "q")
                    break;
                if (rus.Length != 0 && eng.Length != 0)
                    d.Add(eng, rus);
                else break;
            }
        }

        private static void Learning(DictionaryDB d)
        {
            var r = new Random();
            int[] index = Enumerable.Range(1, d.Count).ToArray();
            var i = 0;

            while (index.Length != 0)
            {
                var num = r.Next(0, index.Length);
                i++;

                var rusToEng = r.Next(0, 2);

                Console.Write(i + "\t");
                Console.Write(rusToEng == 1? d[index[num]].Rus : d[index[num]].Eng);
                Console.CursorLeft = 40;
                char answer = Console.ReadKey().KeyChar;
                if (answer == 'q')
                    break;
                if (answer == ' ')
                    d[index[num]].MarkInc();
                Console.CursorLeft = 39;
                Console.Write(" - ");
                Console.WriteLine(rusToEng == 1 ? d[index[num]].Eng : d[index[num]].Rus);

                index = index.Where(x => x != index[num]).ToArray();
            }
            Console.WriteLine();
        }
    }
}
