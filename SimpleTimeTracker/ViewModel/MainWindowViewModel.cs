
using Microsoft.Xaml.Behaviors;
using SimpleTimeTracker.Core;
using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reflection;
using System.Windows;

namespace SimpleTimeTracker.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            this._Model = new TimeTrackerApplication(Path.Combine(GetCurrentAppDirectoryPath(), "storage.json"));
            this._Content = new TimeTrackerApplicationViewModel(this._Model);
        }

        private TimeTrackerApplication _Model;

        /// <summary>
        /// 画面に表示するコンテンツ
        /// </summary>
        public IOnClosing Content
        {
            get => this._Content;
            set => this.SetProperty(ref this._Content, value);
        }
        private IOnClosing _Content;

        private static string GetCurrentAppDirectoryPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        }

    }

    public class MainWindowViewModelClosingBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Closed += this.WindowClosed;
        }

        private void WindowClosed(object? _, EventArgs e)
        {
            (this.AssociatedObject.DataContext as MainWindowViewModel)?.Content.OnClosing();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Closed -= this.WindowClosed;
        }
    }
}
