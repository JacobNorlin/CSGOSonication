using DemoInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Data.SQLite;

namespace CSGOSonification
{
    //For lack of a better name
    class DemoDataParser
    {
        DemoParser parser;
        DBConnection db;

        IObservable<int> roundNumbers;

        public DemoDataParser(string fileName, DBConnection db)
        {
            this.db = db;
            parser = new DemoParser(File.OpenRead(fileName));

            var tickDone = Observable.FromEventPattern<TickDoneEventArgs>(parser, "TickDone")
                .Where(_ =>
                {
                    return parser.PlayingParticipants.ToArray<Player>().Length > 0;
                });

            //createObservables();
            
            

            parser.ParseHeader();
            parser.ParseToEnd();


            

            
        }

        private void createObservables()
        {
            createSmokeEventsObservable();

            roundNumbers = Observable.FromEventPattern<RoundStartedEventArgs>(parser, "RoundStart")
                .Select(_ =>
                {
                    return parser.TScore + parser.CTScore;
                });
        }

        private void createSmokeEventsObservable()
        {
            var weaponFired = Observable.FromEventPattern<WeaponFiredEventArgs>(parser, "WeaponFired");
            var smokesThrown = weaponFired.Where(evt => { return evt.EventArgs.Weapon.Weapon == EquipmentElement.Smoke; })
                .Select(evt =>
                {
                    return Tuple.Create(evt.EventArgs.Shooter.Position, 1);
                });
            var smokesLanded = Observable.FromEventPattern<SmokeEventArgs>(parser, "SmokeNadeStarted")
                .Select(evt =>
                {
                    return Tuple.Create(evt.EventArgs.Position, 0);
                });

            var smokeEvents = smokesThrown.Merge(smokesLanded);
                //.CombineLatest<Tuple<Vector, int>, int, Tuple<Vector,int, int>>(roundNumbers, 
                //(t, rn) => { return Tuple.Create(t.Item1, t.Item2, rn); });

            smokeEvents.Subscribe(t =>
            {
                Console.Out.WriteLine("hej");
                //db.addSmokeEvent(t.Item1, t.Item2);
            });
        }
    }
}
