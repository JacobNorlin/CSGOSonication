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
            //createTables();
        }

        public void createTables()
        {
            var nadeEvents = "create table nadeEvents (posX DECIMAL, posY DECIMAL, startOrLand boolean, nadeType TEXT)";
            var command = new SQLiteCommand(nadeEvents, dbConnection);
            command.ExecuteNonQuery();

            var playerInfo = "create table playerInfo (posX decimal, posY decimal, posZ decimal, armor int, currentEquipmentValue int, disconnected boolean, entityId int, freezeTimeEndEquipmentValue int, hasDefuseKit boolean, hasHelmet boolean, hp int, isAlive boolean, isDucking boolean, lastAliveX decimal, lastAliveY decimal, lastAliveZ decimal, money int, name text, roundStartEquipmentValue int, steamId text, team text, velocityX decimal, velocityY decimal, viewX decimal, viewY decimal, weapon1 text, weapon2 text, weapon3 text, weapon4 text, weapon5 text, currentTime decimal)";
            var cmd = new SQLiteCommand(playerInfo, dbConnection);
            cmd.ExecuteNonQuery();
        }

        public void addNadeEvent(Vector pos, int startedOrLanded, string type)
        {
            
            var sql = "insert into nadeEvents values (" + (int)pos.X + ", " + (int)pos.Y + ", " + startedOrLanded + ", \""+type+"\")";
            Console.Out.WriteLine(sql);
            var cmd = new SQLiteCommand(sql, dbConnection);
            cmd.ExecuteNonQuery();
        }

        public void addPlayerInfo(Player player, float currentTime)
        {
            var sql = "insert into playerInfo values (" +
                      f2s(player.Position.X) + ", " +
                      f2s(player.Position.Y) + ", " +
                      f2s(player.Position.Z) + ", " +
                      player.Armor + ", " +
                      player.CurrentEquipmentValue + ", " +
                      player.Disconnected + ", " +
                      player.EntityID + ", " +
                      player.FreezetimeEndEquipmentValue + ", " +
                      player.HasDefuseKit + ", " +
                      player.HasHelmet + ", " +
                      player.HP + ", " +
                      player.IsAlive + ", " +
                      player.IsDucking + ", " +
                      f2s(player.LastAlivePosition.X) + ", " +
                      f2s(player.LastAlivePosition.Y) + ", " +
                      f2s(player.LastAlivePosition.Z) + ", " +
                      player.Money + ", " +
                      toSqlString(player.Name) + ", " +
                      player.RoundStartEquipmentValue + ", " +
                      player.SteamID + ", " +
                      toSqlString(player.Team.ToString()) + ", " +
                      f2s(player.Velocity.X) + ", " +
                      f2s(player.Velocity.Y) + ", " +
                      f2s(player.Velocity.Z) + ", " +
                      f2s(player.ViewDirectionX) + ", " +
                      f2s(player.ViewDirectionY) + ", ";

            Console.Out.WriteLine(sql);

            var cmd = new SQLiteCommand(sql, dbConnection);
            cmd.ExecuteNonQuery();
            
        }

        private string f2s(float f)
        {
            return f.ToString().Replace(',', '.');
        }
        private string toSqlString(string s)
        {
            return "\"" + s + "\"";
        }
    }
}
