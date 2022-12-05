
using Livet.StatefulModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SimpleTimeTracker.Core
{
    public class TimeTrackerApplication
    {
        public TimeTrackerApplication(string storePath) 
        {
            this._StorePath = storePath;
            this._Switched = new Subject<TimerState>();
            this._Switched.Subscribe(x => this._WantSave = true);
            this._Switched.Throttle(TimeSpan.FromSeconds(10))
                .Where(_ => this._Source.All(x => x.TimerState == TimerState.Stop))
                .Where(_ => this._WantSave)
                .Subscribe(_ => this.Store());

            this._Source.Add(this.SubscribeChild(new TimeTracker()));
        }

        private readonly ObservableSynchronizedCollection<TimeTracker> _Source = new ObservableSynchronizedCollection<TimeTracker>();
        public ReadOnlyNotifyChangedCollection<TimeTracker> Collcetion => _Source.ToSyncedReadOnlyNotifyChangedCollection();

        private Subject<TimerState> _Switched;
        private bool _WantSave = false;

        private string _StorePath;

        private TimeTracker SubscribeChild(TimeTracker child)
        {
            child.Switched.Subscribe(_ =>
            {
                var state = child.TimerState;
                this._Switched.OnNext(state);
                if (state == TimerState.Count)
                {
                    foreach (var item in this._Source.Where(x => x.Id != child.Id).Where(x => x.TimerState == TimerState.Count))
                    {
                        item.Switch();
                    }
                }
            });

            child.Deleted.Subscribe(_ =>
            {
                this._Source.Remove(child);

                if (!this._Source.Any())
                {
                    this._Source.Add(this.SubscribeChild(new TimeTracker()));
                }
            });

            child.MoveRequest.Subscribe(d =>
            {
                if (d)
                {
                    var index = Math.Max(this._Source.IndexOf(child) - 1, 0);
                    this._Source.Remove(child);
                    this._Source.Insert(index, child);
                }
                else
                {
                    var maxLength = this._Source.Count;
                    var index = Math.Min(this._Source.IndexOf(child) + 1, maxLength - 1);
                    this._Source.Remove(child);
                    this._Source.Insert(index, child);
                }
            });

            return child;
        }

        public void AddNew()
        {
            this._Source.Add(this.SubscribeChild(new TimeTracker()));
        }

        public void Store()
        {
            var entities = this._Source.Select(x =>
            {
                return new TimeTrackerEntity((int)x.Time.TotalSeconds, x.Title);
            });
            var str = JsonConvert.SerializeObject(entities);

            File.WriteAllText(this._StorePath, str);
        }

        public void Restore()
        {
            if (!File.Exists(this._StorePath))
            {
                return;
            }

            var str = File.ReadAllText(this._StorePath);

            if (string.IsNullOrWhiteSpace(str))
            {
                return;
            }

            var entities = JsonConvert.DeserializeObject<IEnumerable<TimeTrackerEntity>>(str);

            if (entities == null)
            {
                return;
            }

            this._Source.Clear();
            foreach (var e in entities)
            {
                var tracker = new TimeTracker(e.CountSecond, e.Title);
                this._Source.Add(this.SubscribeChild(tracker));
            }
        }
    }


    public record TimeTrackerEntity(int CountSecond, string Title)
    {
    }
}