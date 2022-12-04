
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SimpleTimeTracker.Core;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SimpleTimeTracker.ViewModel
{
    public class TimeTrackerViewModel
    {
        private readonly TimeTracker _Model;

        public TimeTrackerViewModel(TimeTracker model) 
        {
            this._Model = model;

            this.Title = model.ToReactivePropertyAsSynchronized(x => x.Title)
                .AddTo(this.Disposable);

            this.Time = model.ObserveProperty(x => x.TimeLabel).ToReactiveProperty<string>()
                .AddTo(this.Disposable);

            this.IsFlipped = model.ObserveProperty(x => x.TimerState, isPushCurrentValueAtFirst:true).Select(x => x == TimerState.Count)
                .ToReactiveProperty(mode:ReactivePropertyMode.RaiseLatestValueOnSubscribe)
                .AddTo(this.Disposable);

            this.TimeStateChangedCommand = new ReactiveCommand()
                .WithSubscribe(() => this._Model.Switch())
                .AddTo(this.Disposable);

            this.DeleteCommand = new ReactiveCommand()
               .WithSubscribe(() => this._Model.Delete())
               .AddTo(this.Disposable);

            this.MoveUpCommand = new ReactiveCommand()
                .WithSubscribe(() => this._Model.Move(true))
                .AddTo(this.Disposable);

            this.MoveDownCommand = new ReactiveCommand()
                .WithSubscribe(() => this._Model.Move(false))
                .AddTo(this.Disposable);
        }

        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public ReactiveProperty<string> Title { get; }

        public ReactiveProperty<string> Time { get; }

        public ReactiveProperty<bool> IsFlipped { get; }

        public ReactiveCommand TimeStateChangedCommand { get; set; }

        public ReactiveCommand DeleteCommand { get; set; }

        public ReactiveCommand MoveUpCommand { get; set; }

        public ReactiveCommand MoveDownCommand { get; set; }
    }
}
