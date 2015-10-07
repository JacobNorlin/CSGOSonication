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
    class DataStreamManager
    {
        public DemoParser parser;
        DBConnection db;

        IObservable<int> roundNumbers;
        IObservable<EventPattern<WeaponFiredEventArgs>> weaponFired;
        public IObservable<Tuple<IEnumerable<Player>, float, int>> playerInfoStream;
        public IObservable<Tuple<Vector, Team, int, int, float>> smokeEventStream;
        public IObservable<Tuple<Vector, Team, int, int, float>> flashEventStream;
        public IObservable<EventPattern<HeaderParsedEventArgs>> headerParsedStream;


        public DataStreamManager(string fileName)
        {
            parser = new DemoParser(File.OpenRead(fileName));
            
            createObservables();

            parser.ParseHeader();
            parser.ParseToEnd();
  
        }

        private void createObservables()
        {
            weaponFired = Observable.FromEventPattern<WeaponFiredEventArgs>(parser, "WeaponFired");
            roundNumbers = Observable.FromEventPattern<RoundStartedEventArgs>(parser, "RoundStart")
                .Select(_ =>
                {
                    return parser.TScore + parser.CTScore;
                });
            headerParsedStream = Observable.FromEventPattern<HeaderParsedEventArgs>(parser, "HeaderParsed");

            
            smokeEventStream = createSmokeEventsObservable();
            flashEventStream = createFlashEventObservable();
            playerInfoStream = createPlayerDataObservable();
        }

        private IObservable<Tuple<IEnumerable<Player>, float, int>> createPlayerDataObservable()
        {
            var playerStream = Observable.FromEventPattern<TickDoneEventArgs>(parser, "TickDone")
                .Where(_ =>
                    {
                        return parser.PlayingParticipants.ToArray<Player>().Length > 0;
                    })
                .Select(_ =>
                    {
                        return Tuple.Create(parser.PlayingParticipants, parser.CurrentTime);
                    });

            var playerStreamRn = playerStream.CombineLatest<Tuple<IEnumerable<Player>, float>, int, Tuple<IEnumerable<Player>, float, int>>
                (roundNumbers, (t, rn) =>
                {
                    return Tuple.Create(t.Item1, t.Item2, rn);
                });

            return playerStreamRn;
        }

        private IObservable<Tuple<Vector, Team, int, int, float>> createFlashEventObservable()
        {
            var flashThrown = weaponFired.Where(evt => { return evt.EventArgs.Weapon.Weapon == EquipmentElement.Flash; })
                .Select(evt => { return Tuple.Create(evt.EventArgs.Shooter.Position, evt.EventArgs.Shooter.Team, 1);});
            var flashExploded = Observable.FromEventPattern<NadeEventArgs>(parser, "FlashNadeStarted")
                .Select(evt => { return Tuple.Create(evt.EventArgs.Position, evt.EventArgs.ThrownBy.Team, 0); });
            var flashEvents = flashThrown.Merge(flashExploded)
                .CombineLatest<Tuple<Vector, Team, int>, int, Tuple<Vector, Team, int, int, float>>(roundNumbers,
                (t, rn) => { return Tuple.Create(t.Item1, t.Item2, t.Item3, rn, parser.CurrentTime); });
            return flashEvents;        
        }
        private IObservable<Tuple<Vector, Team, int, int, float>> createSmokeEventsObservable()
        {
            var smokesThrown = weaponFired.Where(evt => { return evt.EventArgs.Weapon.Weapon == EquipmentElement.Smoke; })
                .Select(evt =>
                {
                    var shooter = evt.EventArgs.Shooter;
                    return Tuple.Create(shooter.Position, shooter.Team,  1);
                });
            var smokesLanded = Observable.FromEventPattern<SmokeEventArgs>(parser, "SmokeNadeStarted")
                .Select(evt => {return Tuple.Create(evt.EventArgs.Position, evt.EventArgs.ThrownBy.Team, 0);});
            //Types make this look obtuse. Basically merges all smoke events and appends current round number and time.
            var smokeEvents = smokesThrown.Merge(smokesLanded)
                .CombineLatest<Tuple<Vector, Team, int>, int, Tuple<Vector, Team, int, int, float>>(roundNumbers, 
                (t, rn) => { return Tuple.Create(t.Item1, t.Item2, t.Item3, rn, parser.CurrentTime); });

            return smokeEvents;

        }
    }
}
