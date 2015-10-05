using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using DemoInfo;

namespace CSGOSonification
{
    class DBConnection
    {
        SQLiteConnection dbConnection;
        public DBConnection()
        {
            dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            dbConnection.Open();
        }

        public void createTables()
        {
            var sql = "create table nadeEvents (posX DECIMAL, posY DECIMAL, startOrLand TINYINT, nadeType TEXT)";
            var command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public void addSmokeEvent(Vector pos, int startedOrLanded)
        {
            
            var sql = "insert into nadeEvents values (" + (int)pos.X + ", " + (int)pos.Y + ", " + startedOrLanded + ", \"smoke\")";
            Console.Out.WriteLine(sql);
            var cmd = new SQLiteCommand(sql, dbConnection);
            cmd.ExecuteNonQuery();
        }
    }
}
