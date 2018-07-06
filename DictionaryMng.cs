using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Vocabulary
{
    class DictionaryMng
    {
        public static void CommandAnalyze(DictionaryDB d, string com)
        {
            if (Regex.IsMatch(com, @"^learn\s+\d+"))
                Learning(d, int.Parse(Regex.Match(com, @"^learn\s+(\d+)").Groups[1].Value));
            else if (com == "help")
            {
                Console.Clear();
                try
                {
                    string[] help = File.ReadAllLines("help.txt", Encoding.UTF8);
                    foreach (string x in help)
                        Console.WriteLine(x);
                }
                catch(FileNotFoundException)
                {
                    Console.WriteLine("Файл help.txt не найден");
                }
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
        /// <summary>
        /// Функция повторения слов
        /// </summary>
        /// <param name="d">Словарь слов</param>
        /// <param name="countWords">Кол-во слов для повторения</param>
        private static void Learning(DictionaryDB d, int countWords)
        {
            try
            {
                if (countWords <= 0 || countWords > d.Count)
                    throw new ArgumentException("Недопустимое кол-во слов для повторения");

                Console.WriteLine($"Повторение {countWords}:\n");
                var r = new Random();
                int[] index = Enumerable.Range(1, d.Count).ToArray();
                var i = 0;
                int num = 0;
                int rusToEng = 0;

                while (i < countWords)
                {
                    // выбираем одно из трех слов с наименьшей оценкой
                    int n1 = r.Next(0, index.Length);
                    int n2 = r.Next(0, index.Length);
                    int n3 = r.Next(0, index.Length);
                    num = d[index[n1]].Mark <= d[index[n2]].Mark ? n1 : n2;
                    num = d[index[num]].Mark <= d[index[n3]].Mark ? num : n3;

                    i++;

                    rusToEng = r.Next(0, 2);

                    Console.Write(i + "\t" + (rusToEng == 1 ? d[index[num]].Rus : d[index[num]].Eng));
                    Console.CursorLeft = 40;
                    char answer = Console.ReadKey().KeyChar;
                    if (answer == 'q')
                        break;
                    if (answer == ' ')
                        d[index[num]].MarkInc();
                    else
                        d[index[num]].MarkDec();
                    Console.CursorLeft = 39;
                    Console.WriteLine(" - " + (rusToEng == 1 ? d[index[num]].Eng : d[index[num]].Rus) + "  " + d[index[num]].Mark);

                    index = index.Where(x => x != index[num]).ToArray();
                }
                Console.WriteLine();
            }
            catch(ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
