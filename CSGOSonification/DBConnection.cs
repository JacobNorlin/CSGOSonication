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

            var playerInfo = "create table playerInfo (posX decimal, posY decimal, posZ decimal, armor int, currentEquipmentValue int, disconnected tinyint, entityId int, freezeTimeEndEquipmentValue int, hasDefuseKit boolean, hasHelmet tinyint, hp int, isAlive tinyint, isDucking tinyint, lastAliveX decimal, lastAliveY decimal, lastAliveZ decimal, money int, name text, roundStartEquipmentValue int, steamId text, team text, velocityX decimal, velocityY decimal, velocityZ decimal, viewX decimal, viewY decimal, pistol text, rifle text, currentTime decimal)";
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


        public string addPlayerInfo(Player player, float currentTime)
        {
            var sb = new StringBuilder();

            var sql = sb.Append("insert into playerInfo values (")
                      .Append(f2s(player.Position.X)).Append(", ")
                      .Append(f2s(player.Position.Y)).Append(", ")
                      .Append(f2s(player.Position.Z)).Append(", ")
                      .Append(player.Armor).Append(", ")
                      .Append(player.CurrentEquipmentValue).Append(", ")
                      .Append(Convert.ToInt32(player.Disconnected)).Append(", ")
                      .Append(player.EntityID).Append(", ")
                      .Append(player.FreezetimeEndEquipmentValue).Append(", ")
                      .Append(Convert.ToInt32(player.HasDefuseKit)).Append(", ")
                      .Append(Convert.ToInt32(player.HasHelmet)).Append(", ")
                      .Append(player.HP).Append(", ")
                      .Append(Convert.ToInt32(player.IsAlive)).Append(", ")
                      .Append(Convert.ToInt32(player.IsDucking)).Append(", ")
                      .Append(f2s(player.LastAlivePosition.X)).Append(", ")
                      .Append(f2s(player.LastAlivePosition.Y)).Append(", ")
                      .Append(f2s(player.LastAlivePosition.Z)).Append(", ")
                      .Append(player.Money).Append(", ")
                      .Append(toSqlString(player.Name)).Append(", ")
                      .Append(player.RoundStartEquipmentValue).Append(", ")
                      .Append(player.SteamID).Append(", ")
                      .Append(toSqlString(player.Team.ToString())).Append(", ")
                      .Append(f2s(player.Velocity.X)).Append(", ")
                      .Append(f2s(player.Velocity.Y)).Append(", ")
                      .Append(f2s(player.Velocity.Z)).Append(", ")
                      .Append(f2s(player.ViewDirectionX)).Append(", ")
                      .Append(f2s(player.ViewDirectionY)).Append(", ");
            var weapons = player.Weapons.ToArray<Equipment>();
            for(int i = 0; i < 2; i++) {
                if(i < weapons.Length)
                {
                    sql.Append(toSqlString(weapons[i].Weapon.ToString())).Append(", ");
                }
                else
                {
                    sql.Append(toSqlString("null")).Append(", ");
                }
                
            }
            sql.Append(f2s(currentTime )).Append(")");

            return sb.ToString();
            
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
