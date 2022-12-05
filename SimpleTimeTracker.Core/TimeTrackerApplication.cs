
using Livet.StatefulModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace SimpleTimeTracker.Core
{
    public class TimeTrackerApplication
    {
        public TimeTrackerApplication() 
        {
            this._Source.Add(this.SubscribeChild(new TimeTracker()));
        }

        private readonly ObservableSynchronizedCollection<TimeTracker> _Source = new ObservableSynchronizedCollection<TimeTracker>();
        public ReadOnlyNotifyChangedCollection<TimeTracker> Collcetion => _Source.ToSyncedReadOnlyNotifyChangedCollection();


        private TimeTracker SubscribeChild(TimeTracker child)
        {
            child.Switched.Subscribe(_ =>
            {
                var state = child.TimerState;
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

        public void Store(string path)
        {
            var entities = this._Source.Select(x =>
            {
                return new TimeTrackerEntity((int)x.Time.TotalSeconds, x.Title);
            });
            var str = JsonConvert.SerializeObject(entities);

            File.WriteAllText(path, str);
        }

        public void Restore(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var str = File.ReadAllText(path);

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