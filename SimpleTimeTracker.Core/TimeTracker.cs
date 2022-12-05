using Reactive.Bindings.Extensions;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

namespace SimpleTimeTracker.Core
{
    public class TimeTracker : BindableBase
    {
        public TimeTracker() : this(0, "タスク")
        {

        }

        public TimeTracker(int countSecond, string title)
        {
            this._Title = title;
            this._Time = TimeSpan.FromSeconds(countSecond);

            this._TimeLabel = this.TimeToString(this.Time);

            this._Switched = new Subject<Unit>().AddTo(this.Disposable);

            this._Deleted = new Subject<Unit>().AddTo(this.Disposable);

            this._MoveRequest = new Subject<bool>().AddTo(this.Disposable);

            this.ObserveProperty(x => x.TimerState).Subscribe(x =>
            {
                if (x == TimerState.Count)
                {
                    _Watch.Start();
                    var interval = Observable.Interval(TimeSpan.FromMilliseconds(100));
                    var disposable = interval.Subscribe(
                    i =>
                    {
                        this.TimeLabel = this.TimeToString(this.Time + this._Watch.Elapsed);
                    });

                    this._StopAction = new Action(() =>
                    {
                        disposable.Dispose();
                        _Watch.Stop();
                        this.Time += this._Watch.Elapsed;
                        _Watch.Reset();
                    });
                    this.TimeLabel = this.TimeToString(this.Time + this._Watch.Elapsed);
                }
                else
                {
                    this._StopAction();
                    this._StopAction = delegate { };
                    this.TimeLabel = this.TimeToString(this.Time);
                }
            });
        }

        private Guid _Id = Guid.NewGuid();
        public Guid Id
        {
            get => this._Id;
            private set => this.SetProperty(ref this._Id, value);
        }

        private string _Title;
        public string Title
        {
            get => this._Title;
            private set => this.SetProperty(ref this._Title, value);
        }

        private Stopwatch _Watch = new Stopwatch();
        private Action _StopAction = delegate { };

        private TimerState _TimerState = TimerState.Stop;
        public TimerState TimerState
        {
            get => this._TimerState;
            private set => this.SetProperty(ref this._TimerState, value);
        }

        private TimeSpan _Time;
        public TimeSpan Time
        {
            get => this._Time;
            private set => this.SetProperty(ref this._Time, value);
        }

        private string _TimeLabel;
        public string TimeLabel
        {
            get => this._TimeLabel;
            private set => this.SetProperty(ref this._TimeLabel, value);
        }

        public IObservable<Unit> Switched => this._Switched;
        private readonly Subject<Unit> _Switched;

        public IObservable<Unit> Deleted => this._Deleted;
        private readonly Subject<Unit> _Deleted;

        public IObservable<bool> MoveRequest => this._MoveRequest;
        private readonly Subject<bool> _MoveRequest;

        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public void Switch()
        {
            if (this.TimerState == TimerState.Count)
            {
                this.TimerState = TimerState.Stop;
            }
            else
            {
                this.TimerState = TimerState.Count;
            }

            this._Switched.OnNext(Unit.Default);
        }

        private string TimeToString(TimeSpan time)
        {
            if (this.TimerState == TimerState.Stop)
            {
                return (time.Hours + ((time.Minutes % 60) / (decimal)60)).ToString("F1");
            }
            else
            {
                return time.ToString("hh\\:mm\\:ss");
            }
        }

        public void Delete()
        {
            var res = MessageBox.Show($"タイマー（{this.Title}）を削除しますか？", "タイマーの削除", MessageBoxButton.OKCancel);

            if (res != MessageBoxResult.OK)
            {
                return;
            }

            this._Deleted.OnNext(Unit.Default);
            this.Disposable.Dispose();
        }

        public void Move(bool v)
        {
            this._MoveRequest.OnNext(v);
        }

        public void AddTime(double v)
        {
            if (this._TimerState == TimerState.Count)
            {
                return;
            }

            this.Time += TimeSpan.FromHours(v);
            this.TimeLabel = this.TimeToString(this.Time);
        }
    }

    public enum TimerState
    {
        Stop,
        Count,
    }
}
