using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace Vocabulary
{
    [DataContract]
    class Word
    {
        [DataMember]
        private string _rus;
        [DataMember]
        private string _eng;
        [DataMember]
        private int _mark;
        /// Делегат вывода
        private delegate void _OutputDelegate(string message);
        private event _OutputDelegate Print;

        /// Возвращает русское слово
        public string Rus => this._rus;

        /// Возвращает английское слово
        public string Eng => this._eng; 

        /// Возвращает оценку
        public int Mark => this._mark;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="eng">Английское слово</param>
        /// <param name="rus">Русское слово</param>
        /// <param name="mark">Оценка</param>
        public Word(string eng, string rus, int mark)
        {
            Print += IOWindow.Print;
            try
            {
                if (eng.Length > 0 && rus.Length > 0)
                {
                    this._eng = eng;
                    this._rus = rus;
                    this._mark = mark;
                }
                else
                    throw new ArgumentException();
            }
            catch (ArgumentException e)
            {
                Print("В конструктор класса Word переданы неверные параметры\n");
                Print(e.StackTrace + "\n");
            }
        }

        /// Повышение оценки
        public void MarkInc()
        {
            if (this._mark < 5)
                this._mark++;
        }

        /// Понижение оценки
        public void MarkDec()
        {
            if (this._mark > 0)
                this._mark--;
        }
    }
}
