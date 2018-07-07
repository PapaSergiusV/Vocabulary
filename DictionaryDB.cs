using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace Vocabulary
{
    [DataContract]
    class DictionaryDB// : Dictionary<int, Word>
    {
        [DataMember]
        private Dictionary<int, Word> _words;
        [DataMember]
        private int _count;
        [DataMember]
        private int[] _markCount;
        /// Делегат вывода
        private delegate void _OutputDel(string message);
        /// Делегат считывания строки
        private delegate string _ReadStrDel();
        /// Делегат Да / Нет
        private delegate bool _ReadYesNoDel();
        /// События к делегатам
        private event _OutputDel Write;
        private event _ReadStrDel ReadLine;
        private event _ReadYesNoDel ReadYesNo;

        /// <summary>
        /// Возвращает слово по его номеру в словаре
        /// </summary>
        /// <param name="num">Номер</param>
        /// <returns>Ссылка на объект Word</returns>
        public Word this[int num]
        {
            get
            {
                if (num > 0 && num <= this._count)
                    return this._words[num];
                return null;
            }
        }
        /// Кол-во слов в словаре
        public int Count => this._count;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">Ссылка на файл словарь.txt</param>
        public DictionaryDB(string path)
        {
            Write += IOWindow.Print;
            ReadYesNo += IOWindow.ReadYesNo;
            ReadLine += IOWindow.ReadString;

            this._markCount = new int[6];
            this._words = new Dictionary<int, Word>();

            if (File.Exists(path))
            {
                string[] line = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 0; i < line.Length; i++)
                {
                    if (Regex.IsMatch(line[i], @"^\w+.+\s\|\s\w"))
                    {
                        var m = Regex.Match(line[i], @"^(\w+.+)\s\|\s(.+)\s\|\s(\d+)$");
                        int mark = int.Parse(m.Groups[3].Value);
                        this._Load(m.Groups[2].Value, m.Groups[1].Value, mark);
                        this._markCount[mark]++;
                    }
                    else
                        Write($"Line {i + 1} is not match pattern\n");
                }
            }
            else
                Write($"! File not found ({path})\n");
        }

        /// <summary>
        /// Загрузка считываемых слов в словарь класса. Используется для конструктора
        /// </summary>
        /// <param name="eng">Английское слово</param>
        /// <param name="rus">Русское слово</param>
        /// <param name="mark">Оценка</param>
        private void _Load(string eng, string rus, int mark)
        {
            this._count++;
            (rus, eng) = this._WordFormat(rus, eng);
            this._words.Add(this._count, new Word(eng, rus, mark));
        }

        /// <summary>
        /// Добавление слова в словарь класса
        /// </summary>
        /// <param name="eng">Английское слово</param>
        /// <param name="rus">Русское слово</param>
        /// <param name="mark">Оценка</param>
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
        /// <summary>
        /// Редактирование слова из словаря
        /// </summary>
        /// <param name="eng">Английское слово</param>
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

        /// <summary>
        /// Удаление слова из словаря
        /// </summary>
        /// <param name="eng">Английское слово</param>
        public void Remove(string eng)
        {
            KeyValuePair<int, Word> toDel = this._Search(eng);

            if (toDel.Key != -1)
                if (this._words.Remove(toDel.Key))
                    Write($"Слово {eng} удалено\n");
        }
        /// <summary>
        /// Поиск слова в словаре
        /// </summary>
        /// <param name="eng">Английское слово</param>
        /// <returns>KeyValuePair из словаря либо -1 и null</returns>
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

        /// <summary>
        /// Функция для форматирования слова
        /// </summary>
        /// <param name="rus">Русское слово</param>
        /// <param name="eng">Английское слово</param>
        /// <returns>Отформатированное слово</returns>
        private (string, string) _WordFormat(string rus, string eng)
        {
            rus = rus.Trim(' ', ',', '.', ':', ';', '/');
            eng = eng.Trim(' ', ',', '.', ':', ';', '/', '[', ']');
            return (char.ToUpper(rus[0]) + rus.Substring(1).ToLower(), eng.ToLower());
        }

        /// <summary>
        /// Сохранение словаря в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public void Save(string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                foreach (KeyValuePair<int, Word> line in this._words)
                    sw.WriteLine(line.Value.Rus + " | " + line.Value.Eng + " | " + line.Value.Mark);
            }
        }

        /// <summary>
        /// Возвращает кол-во слов с оценкой
        /// </summary>
        /// <param name="num">Оценка</param>
        /// <returns>Кол-во слов</returns>
        public int MarkCount(int num)
        {
            try
            {
                return this._markCount[num];
            }
            catch(IndexOutOfRangeException e)
            {
                Write($"Оценки {num} не существует!\n{e.StackTrace}\n");
                return 0;
            }
        }
    }
}
