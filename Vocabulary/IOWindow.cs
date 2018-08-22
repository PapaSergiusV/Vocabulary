using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocabulary
{
    class IOWindow
    {
        static public void Print(string message)
        {
            Console.Write(message);
        }

        static public bool ReadYesNo()
        {
            return Console.ReadKey().KeyChar != 'n';
        }

        static public string ReadString()
        {
            return Console.ReadLine();
        }
    }
}
