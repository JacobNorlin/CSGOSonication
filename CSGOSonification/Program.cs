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

        DBConnection db;
        DemoParser parser;
        DataStreamManager dataStreams;
        static void Main(string[] args)
        {



            Program p = new Program();
            p.parseToSQLite();




            Console.In.ReadLine();
        }



        public Program()
        {
            db = new DBConnection();
            parser = new DemoParser(File.OpenRead("test.dem"));
            //db.createTables();
            //in the end ex was kind of useless, but it was fun to play with.
            dataStreams = new DataStreamManager(parser);


        }

        public void parseToSQLite()
        {
            setSqlSubscription();

            parser.ParseHeader();
            parser.ParseToEnd();
        }

        public void setSqlSubscription()
        {
            dataStreams.headerParsedStream.Subscribe(_ =>
            {
                db.beginTransaction();
            });

            //transform each element to an array of sql statements
            var playerInfoSQL = dataStreams.playerInfoStream.
                                    Select(t =>
                                    {
                                        var players = t.Item1;
                                        return players.Select(p =>
                                        {
                                            return db.playerInfoToSQL(p, t.Item2, t.Item3);
                                        });
                                    });

            var flashSql = dataStreams.flashEventStream.
                                    Select(evt =>
                                    {
                                        return db.nadeEventToSQL(evt.Item1, evt.Item2.ToString(), evt.Item3, evt.Item4, evt.Item5, "flash");
                                    });
            var smokeSql = dataStreams.smokeEventStream.
                                    Select(evt =>
                                    {
                                        return db.nadeEventToSQL(evt.Item1, evt.Item2.ToString(), evt.Item3, evt.Item4, evt.Item5, "smoke");
                                    });

            playerInfoSQL.Subscribe(sqlStatements =>
            {
                Console.Out.WriteLine(parser.ParsingProgess);
                foreach (var sql in sqlStatements)
                {
                    db.executeSql(sql);
                }

                //Since there is no event of the parsing finishin
                if (parser.ParsingProgess == 1)
                {
                    db.endTransaction();
                }
            });

            flashSql.Subscribe(sql => { db.executeSql(sql); });
            smokeSql.Subscribe(sql => { db.executeSql(sql); });
        }

    }
}

