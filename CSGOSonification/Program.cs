using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
namespace CSGOSonification {

    class Program
    {
        static void Main(string[] args)
        {
            var db = new DBConnection();
            //db.createTables();

            var dataParser = new DemoDataParser("test.dem", db);
            Console.In.ReadLine();
        }

    }
}

