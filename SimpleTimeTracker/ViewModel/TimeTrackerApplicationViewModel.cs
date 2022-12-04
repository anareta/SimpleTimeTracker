using Reactive.Bindings;
using SimpleTimeTracker.Core;
using System.IO;
using System.Reflection;

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
            var path = GetCurrentAppDirectoryPath();
            this._Model.Store(Path.Combine(path, "storage.json"));
        }

        public void OnStarting()
        {
            var path = GetCurrentAppDirectoryPath();
            this._Model.Restore(Path.Combine(path, "storage.json"));
        }

        public static string GetCurrentAppDirectoryPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        }
    }
}
