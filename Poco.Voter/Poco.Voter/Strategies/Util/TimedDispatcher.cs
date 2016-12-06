using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace Poco.Voter.Strategies.Util
{
    class TimedDispatcher : IDisposable
    {
        BlockingCollection<Action> CallbackQueue = new BlockingCollection<Action>();
        Dictionary<Action, Timer> Timers = new Dictionary<Action, Timer>();

        public void Add(Action action, int dueTime, int period)
        {
            Timers[action] = new Timer(
                state => CallbackQueue.Add(action),
                null,
                dueTime,
                period);
        }

        public void Remove(Action action)
        {
            Timer timer;
            if (!Timers.TryGetValue(action, out timer)) return;

            // Stop the timer - we do this by making it never do any work again.
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            Timers.Remove(action);
        }

        public void Run(CancellationToken cancel)
        {
            try
            {
                while (true)
                {
                    var cb = CallbackQueue.Take(cancel);

                    try
                    {
                        cb();
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Error during callback. {0}", e);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        #region IDisposable Members

        /// <summary>
        /// Internal variable which checks if Dispose has already been called
        /// </summary>
        bool disposed;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Tell the garbage collector that the object doesn't require any
            // cleanup when collected since Dispose was called explicitly.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// </summary>
        /// <param name="disposing"><c>true</c> the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.; <c>false</c> the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                try
                {
                    // Stop all timers.
                    foreach (var t in Timers.Values)
                        t.Change(Timeout.Infinite, Timeout.Infinite);
                }
                catch (Exception) { }
                // Managed cleanup code here, while managed refs still valid
            }
            // Unmanaged cleanup code here

            disposed = true;
        }

        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method 
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        ~TimedDispatcher()
        {
            Dispose(false);
        }

        #endregion
    }
}
