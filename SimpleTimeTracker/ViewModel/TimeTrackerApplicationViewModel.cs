using Reactive.Bindings;
using SimpleTimeTracker.Core;

namespace SimpleTimeTracker.ViewModel
{
    public class TimeTrackerApplicationViewModel : BindableBase, IOnClosing
    {
        private TimeTrackerApplication _Model;

        public TimeTrackerApplicationViewModel(TimeTrackerApplication model)
        {
            this._Model = model;

            this.TimeTrackers = this._Model.Collcetion.ToReadOnlyReactiveCollectionSafe(x => new TimeTrackerViewModel(x));
            this.AddNewCommand = new ReactiveCommand().WithSubscribe(() => this._Model.AddNew());

            this.OnStarting();
        }

        public ReadOnlyReactiveCollection<TimeTrackerViewModel> TimeTrackers { get; }

        public ReactiveCommand AddNewCommand { get; set; }

        public void OnClosing()
        {
            this._Model.Store();
        }

        public void OnStarting()
        {
            this._Model.Restore();
        }
    }
}
