using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Vocabulary
{
    class DictionaryDB
    {
        private Dictionary<int, Word> _words;
        private int _count;
        private delegate void _OutputDel(string message);
        private delegate string _ReadStrDel();
        private delegate bool _ReadYesNoDel();
        private event _OutputDel Write;
        private event _ReadStrDel ReadLine;
        private event _ReadYesNoDel ReadYesNo;

        public Word this[int num]
        {
            get
            {
                if (num > 0 && num <= this._count)
                    return this._words[num];
                return null;
            }
        }
        public int Count
        {
            get { return this._count; }
        }

        public DictionaryDB(string path)
        {
            Write += IOWindow.Print;
            ReadYesNo += IOWindow.ReadYesNo;
            this._words = new Dictionary<int, Word>();
            if (File.Exists(path))
            {
                string[] line = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 0; i < line.Length; i++)
                {
                    if (Regex.IsMatch(line[i], @"^\w+.+\s\|\s\w"))
                    {
                        var m = Regex.Match(line[i], @"^(\w+.+)\s\|\s(.+)\s\|\s(\d+)$");
                        this._Load(m.Groups[2].Value, m.Groups[1].Value, int.Parse(m.Groups[3].Value));
                    }
                    else
                        Write($"Line {i + 1} is not match pattern\n");
                }
            }
            else
                Write($"! File not found ({path})\n");
        }

        private void _Load(string eng, string rus, int mark)
        {
            this._count++;
            (rus, eng) = this._WordFormat(rus, eng);
            this._words.Add(this._count, new Word(eng, rus, mark));
        }

        public void Add(string eng, string rus, int mark = 0)
        {
            try
            {
                if (this._words.FirstOrDefault(x => x.Value.Eng == eng).Value != null)
                    throw new ApplicationException($"! Слово {eng} существует");
                this._count++;
                (rus, eng) = this._WordFormat(rus, eng);
                Write($"Сохранить: '{rus} | {eng}' ? Введите enter/n: ");
                if (ReadYesNo())
                    this._words.Add(this._count, new Word(eng, rus, mark));
                Write("\n");
            }
            catch(ApplicationException e)
            {
                Write(e.Message + "\n");
            }
        }

        public void Edit(string eng)
        {
            KeyValuePair<int, Word> toEdit = this._Search(eng);

            if (toEdit.Key != -1)
            {
                Write($"Edit: {toEdit.Value.Rus} | {toEdit.Value.Eng}\nВведите англ. слово: ");
                eng = ReadLine();
                Write("Введите рус. слово:  ");
                string rus = ReadLine();

                if (eng.Length == 0 || rus.Length == 0)
                {
                    Write("! Некорректный ввод\n");
                    return;
                }

                (rus, eng) = this._WordFormat(rus, eng);

                Write($"Сохранить: '{rus} | {eng}' ? Введите enter/n: ");
                if (ReadYesNo())
                    this._words[toEdit.Key] = new Word(eng, rus, 0);
                Write("\n");
            }
        }

        public void Remove(string eng)
        {
            KeyValuePair<int, Word> toDel = this._Search(eng);

            if (toDel.Key != -1)
                this._words.Remove(toDel.Key);
        }

        private KeyValuePair<int, Word> _Search(string eng)
        {
            try
            {
                KeyValuePair<int, Word> res = this._words.FirstOrDefault(x => x.Value.Eng == eng);
                if (res.Value == null)
                    throw new ApplicationException($"! Слова {eng} не существует");
                return res;
            }
            catch (ApplicationException e)
            {
                Write(e.Message + "\n");
                return new KeyValuePair<int, Word>(-1, null);
            }
        }

        private (string, string) _WordFormat(string rus, string eng)
        {
            rus = rus.Trim(' ', ',', '.', ':', ';', '/');
            eng = eng.Trim(' ', ',', '.', ':', ';', '/', '[', ']');
            return (char.ToUpper(rus[0]) + rus.Substring(1).ToLower(), eng.ToLower());
        }

        public void Save(string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                foreach (KeyValuePair<int, Word> line in this._words)
                    sw.WriteLine(line.Value.Rus + " | " + line.Value.Eng + " | " + line.Value.Mark);
            }
        }

        public void Print()
        {
            foreach (KeyValuePair<int, Word> word in this._words)
                Write($"{word.Key}\t{word.Value.Rus} - {word.Value.Eng}\n");
        }
    }
}
