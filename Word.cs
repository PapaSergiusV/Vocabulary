using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocabulary
{
    class Word
    {
        private string _rus;
        private string _eng;
        private int _mark;
        private delegate void _InputDelegate(string message);
        private event _InputDelegate Print;

        public string Rus
        {
            get { return this._rus; }
        }
        public string Eng
        {
            get { return this._eng; }
        }
        public int Mark
        {
            get { return this._mark; }
        }

        public void MarkInc()
        {
            this._mark++;
        }

        public void MarkDec()
        {
            if (this._mark > 0)
                this._mark--;
        }

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
            catch(ArgumentException e)
            {
                Print("В конструктор класса Word переданы неверные параметры\n");
                Print(e.StackTrace + "\n");
            }
        }
    }
}
