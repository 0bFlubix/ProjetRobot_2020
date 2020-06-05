using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UI;
using System.Windows;

namespace Console
{
    class ConsoleLogic
    {
        UI.MainWindow BOT_UI = new UI.MainWindow();
        Thread UI_Caller = new Thread(new ThreadStart(BOT_UI.InitializeComponent()));

        public void test()
        {
        }
        static void Main(string[] args)
        {
            
        }
    }
}
