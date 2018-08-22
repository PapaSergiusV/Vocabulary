using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Vocabulary
{
    class DictDB
    {
        private Dictionary<int, Word> _words;

        public DictDB(string path)
        {
            if (File.Exists(path))
            {
                string[] line = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 0; i < line.Length; i++)
                {
                    if (Regex.IsMatch(line[i], @"^\w+.+\s\|\s\w"))
                    {
                        var m = Regex.Match(line[i], @"^(\w+.+)\s\|\s(.+)$");
                        this._words.Add(i + 1, new Word(m.Groups[2].Value, m.Groups[1].Value, 0));
                    }
                    else
                        Console.WriteLine($"Line {i + 1} is not match pattern");
                }
            }
            else
                Console.WriteLine($"File not found ({path})");
        }

        public void Print()
        {
            foreach (KeyValuePair<int, Word> word in this._words)
                Console.WriteLine($"{word.Key}\t{word.Value.Rus} - {word.Value.Eng}");
        }
    }
}
