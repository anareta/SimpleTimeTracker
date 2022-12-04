using Livet.StatefulModel;
using Reactive.Bindings;
using System.Reactive.Concurrency;

namespace SimpleTimeTracker.Core
{
    /// <summary>
    /// スレッドセーフなReadOnlyReactiveCollection生成処理
    /// </summary>
    public static class ToReadOnlyReactiveCollectionSafeExtension
    {
        /// <summary>
        /// convert ObservableSynchronizedCollection to ReadOnlyReactiveCollection with thread safe
        /// </summary>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollectionSafe<T>(
            this ObservableSynchronizedCollection<T> self, IScheduler? scheduler = null, bool disposeElement = true)
            where T : class
        {
            return self.ToReadOnlyReactiveCollectionSafe(x => x, scheduler, disposeElement);
        }

        /// <summary>
        /// convert ObservableSynchronizedCollection to ReadOnlyReactiveCollection with thread safe
        /// </summary>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollectionSafe<T, U>(
            this ObservableSynchronizedCollection<T> self, Func<T, U> converter, IScheduler? scheduler = null,
            bool disposeElement = true)
        {
            lock (self.Synchronizer.LockObject)
            {
                return self.ToReadOnlyReactiveCollection<T, U>(self.ToCollectionChanged<T>(), converter, scheduler, disposeElement);
            }
        }

        /// <summary>
        /// convert ReadOnlyNotifyChangedCollection to ReadOnlyReactiveCollection with thread safe
        /// </summary>
        public static ReadOnlyReactiveCollection<T> ToReadOnlyReactiveCollectionSafe<T>(
            this ReadOnlyNotifyChangedCollection<T> self, IScheduler? scheduler = null, bool disposeElement = true)
            where T : class
        {
            return self.ToReadOnlyReactiveCollectionSafe(x => x, scheduler, disposeElement);
        }

        /// <summary>
        /// convert ReadOnlyNotifyChangedCollection to ReadOnlyReactiveCollection with thread safe
        /// </summary>
        public static ReadOnlyReactiveCollection<U> ToReadOnlyReactiveCollectionSafe<T, U>(
            this ReadOnlyNotifyChangedCollection<T> self, Func<T, U> converter, IScheduler? scheduler = null,
            bool disposeElement = true)
        {
            lock (self.SourceCollection.Synchronizer.LockObject)
            {
                return ((IEnumerable<T>)self).ToReadOnlyReactiveCollection(
                    self.ToCollectionChanged<T>(),
                    converter,
                    scheduler,
                    disposeElement);
            }
        }
    }
}
