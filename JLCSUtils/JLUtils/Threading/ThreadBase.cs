using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JohnLambe.Util.Threading
{
    /// <summary>
    /// Base class for threads, which encapsulates the method which runs in the thread.
    /// </summary>
    public class ThreadBase : NotifyOnDispose<ThreadBase>
    {
        protected enum ThreadControlState
        {
            None,
            /// <summary>
            /// The thread is running and not requested to stop.
            /// </summary>
            Running,
            /// <summary>
            /// The thread has been requested to stop.
            /// </summary>
            Stopping,
            /// <summary>
            /// The thread has terminated.
            /// </summary>
            Stopped,
            /// <summary>
            /// Thread is running and a safe 'suspend' (request to sleep when safe) has been requested.
            /// </summary>
            Suspending,
            /// <summary>
            /// Thread is in a safe 'suspended' state (sleeping).
            /// </summary>
            Suspended,
            /// <summary>
            /// Thread is in a safe 'suspended' state (sleeping) and has been requested to resume.
            /// </summary>
            Resuming
        };

        public ThreadBase()
        {
            ThreadInterval = 1000;
            Priority = ThreadPriority.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        public ThreadBase(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// Start the thread.
        /// If it is already running, this stops it and restarts it.
        /// </summary>
        protected virtual void StartThread()
        {
            StopThread();    //TODO: Don't call StopThread if not running - counter-intuitive for overridding StopThread implementations.
                // Have to wait for it to stop because otherwise, the old thread could set _controlState to Stopped after the new one starts.
            //TODO?: Could use a state that requests it to stop and not update _controlState.
                // The overridden implementation may do things that are not safe with two instances running.

            _thread = new Thread(new ThreadStart(ThreadExecute));
            _thread.Priority = Priority;
            if(Name != null)
                _thread.Name = Name;

            Thread.MemoryBarrier();
            _thread.Start();
        }

        /// <summary>
        /// The method that is run in the thread.
        /// </summary>
        protected virtual void ThreadExecute()
        {
            ThreadControlState controlState = ThreadControlState.Running;
            _controlState.Value = controlState;

            bool continueRunning = true;
            
            //try
            continueRunning = ThreadStarting();
            //TODO: call ThreadException on exception
            //  continueRunning = ThreadException();

            do
            {
                if (controlState == ThreadControlState.Running)
                {
                    try
                    {
                        continueRunning = MainLoop();
                        Thread.Sleep(ThreadInterval);
                    }
                    catch (ThreadInterruptedException)
                    {   // Could have been interrupted in the Sleep call above, or while in a Wait or Join state within MainLoop().
                        // Ignore the exception. It will continue or exit depending on the _stopState state.
                        // (The thread can be made to exit safely by setting this, then calling Interupt).
                    }
                    catch(Exception ex)
                    {
                        if(!ThreadException(ex))
                        {
                            throw;
                        }
                    }
                    //TODO: call ThreadException on exception
                }

//                _controlState.ConditionalUpdate(ThreadStopState.Suspending, ThreadStopState.Suspended, ref stopState);
                // Whether interrupted or not, read the state before the end of this loop.
                // If suspending, update the state to Suspended. The thread will sleep below.

                
                lock(_controlState)
                {
                    controlState = _controlState.UnsafeValue; 
                    if (controlState == ThreadControlState.Suspending)
                    {
                        _controlState.UnsafeValue = ThreadControlState.Suspended;  // update the state.
                        controlState = ThreadControlState.Suspended;               // The thread will sleep when the lock on the state is released
                    }
                    else if (controlState == ThreadControlState.Resuming)
                    {
                        _controlState.UnsafeValue = ThreadControlState.Running;  // update the state.
                        controlState = ThreadControlState.Running;         
                    }
                }
                if (controlState == ThreadControlState.Suspended)
                {
                    try
                    {
                        Thread.Sleep(System.Threading.Timeout.Infinite);  // sleep until interrupted
                    }
                    catch(ThreadInterruptedException)
                    {
/*
                        lock (_stopState)
                        {
                            stopState = _stopState.UnsafeValue;
                            if (stopState == ThreadStopState.Resuming)
                            {   // resume
                                _stopState.UnsafeValue = ThreadStopState.Running;
                                stopState = ThreadStopState.Running;
                            }
                        }
 */
                    }
                    controlState = _controlState.Value;
                    // Could be Stopping (loop will exit), Resuming (will change to Running on next iteration), or still Suspended (will return here on next iteration).
                }

//                _stopState.ConditionalUpdate(ThreadStopState.Resuming, ThreadStopState.Running, ref stopState);
/*
                stopState = _stopState; // whether interrupted or not, read the state before the end of this loop.
                if (stopState == ThreadStopState.Suspending)
                {
                    if (_stopState.ConditionalUpdate(ThreadStopState.Suspending, ThreadStopState.Suspended))
                    {   // if still suspending
                        Thread.Sleep(System.Threading.Timeout.Infinite);
                    }
                }
*/

            } while (controlState != ThreadControlState.Stopping
                && controlState != ThreadControlState.Stopped
                && continueRunning);
            //TODO: Option to synchronize on _stopState less often (except when Interrupted)?

            ThreadStopping();

            _controlState.Value = ThreadControlState.Stopped;   // flag as stopped
        }

        #region Methods for overriding, called within thread

        protected virtual bool ThreadStarting()
        {
            return true;
        }

        /// <summary>
        /// Called periodically in the thread (by ThreadExecute).
        /// </summary>
        protected virtual bool MainLoop()
        {
            return true;
        }

        protected virtual void ThreadStopping()
        {
        }

        /// <summary>
        /// Called when an exception occurs within this thread, that is not caught in a subclass of this.
        /// </summary>
        /// <param name="ex">The exception that occurred in this thread.</param>
        /// <exception></exception>
        /// <returns>true to continue running the thread. false to rethrow the exception.</returns>
        protected virtual bool ThreadException(Exception ex)
        {
            return false;
        }

        #endregion

        #region Control from within the thread

        /// <summary>
        /// Stop the thread after the ThreadMain method returns.
        /// </summary>
        protected virtual void InternalStop()
        {
            _controlState.Value = ThreadControlState.Stopping;
        }

        /// <summary>
        /// Suspend the thread after the ThreadMain method returns.
        /// </summary>
        protected virtual void InternalSuspend()
        {
            _controlState.Value = ThreadControlState.Suspending;
        }

        #endregion

        #region Control from other thread

        /// <summary>
        /// Make this thread stop, while not in MainLoop().
        /// </summary>
        /// <param name="wait"></param>
        protected virtual bool StopThread(bool wait = true)
        {
            bool result = false;
            if (_thread != null)
            {
                // Old State            New State
                // ---------            ---------
                // None                 None
                // Running              Stopping
                // Stopping             Stopping
                // Stopped              Stopped  - Do nothing
                // Suspended            Stopping - and interrupt: resumes and stops
                // Suspending           Stopping (cancel Suspend request)
                // Resuming             Stopping - resumes and stops

                ThreadControlState controlState;
                lock (_controlState)
                {
                    controlState = _controlState.Value;
                    switch(controlState)
                    {
                        case ThreadControlState.None:
                            break;
                        case ThreadControlState.Stopped:
                            break;  // do nothing - it remains stopped
                        default:
                            _controlState.Value = ThreadControlState.Stopping;
                            controlState = ThreadControlState.Stopping;
                            break;
                    }
                }
                //                if (_controlState.ConditionalUpdate(ThreadControlState.Running, ThreadControlState.Stopping)) // if it was Running (have to check this, before we call try to Interrupt and Join).
                if(controlState == ThreadControlState.Stopping)
                {   
                    _thread.Interrupt();
                    if (wait)
                    {
                        _thread.Join();
                        /*                            while (_stopState != ThreadStopState.Stopped)   // wait for the thread to indicate that it has stopped
                                                    {
                                                        Thread.Sleep(10);
                                                    }   //TODO: Pass _thread a reference to this thread and have it Interrupt this one?
                         */
                        result = true;
                    }
                }
                _thread = null;
            }
            return result;
        }

        /// <summary>
        /// Make the thread sleep when it is in a state in which it can do so.
        /// </summary>
        /// <param name="wait"></param>
        /// <returns>true iff the thread is known to be in the 'suspended' state.</returns>
        protected virtual bool SuspendThread(bool wait = true)
        {
            if (_thread != null)
            {
                if (_controlState.ConditionalUpdate(ThreadControlState.Running, ThreadControlState.Suspending))
                {   // if it was Running (have to check this, before we call try to Interrupt and Join).
                    _thread.Interrupt();
                    if (wait)
                    {
                        _controlState.WaitForValue(ThreadControlState.Suspended);
/*
                        while (_stopState != ThreadStopState.Stopped)   // wait for the thread to indicate that it has stopped
                        {
                            Thread.Sleep(10);
                        }   //TODO: Pass _thread a reference to this thread and have it Interrupt this one?
                        return true;
 */
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wait"></param>
        /// <returns>true iff the thread is known to be in the 'running' state.</returns>
        protected virtual bool ResumeThread(bool wait = true)
        {
            //TODO: If Suspending, change to Running (cancel the Suspend request)

            if (_thread != null)
            {
                if (_controlState.ConditionalUpdate(ThreadControlState.Suspended, ThreadControlState.Running))
                {   // if it was Running (have to check this, before we call try to Interrupt and Join).
                    _thread.Interrupt();
                    if (wait)
                    {
                        _controlState.WaitForValue(ThreadControlState.Running);
                        /*
                        do
                        {
                            Thread.Sleep(10);
                        } while (_stopState != ThreadStopState.Running);   // wait for the thread to indicate that it has stopped
                        */
                            //TODO?: Pass _thread a reference to this thread and have it Interrupt this one?
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            StopThread();    // stop the thread on disposing this object
        }

        #endregion

        /// <summary>
        /// The name of the thread (if not null).
        /// </summary>
        protected virtual string Name { get; set; }
        
        /// <summary>
        /// Priority of the thread.
        /// </summary>
        protected virtual ThreadPriority Priority { get; set; }

        /// <summary>
        /// Interval at which <see cref="MainLoop"/> is called.
        /// Actually the idle time between calls.
        /// </summary>
        protected virtual int ThreadInterval { get; set; }

        /// <summary>
        /// Reference to the thread.
        /// </summary>
        protected Thread _thread = null;
        /// <summary>
        /// Thread-safe variable for communication between the thread which this class is used in
        /// and the thread wrapped/controlled by this class.
        /// </summary>
        protected ThreadSafeExt<ThreadControlState> _controlState = new ThreadSafeExt<ThreadControlState>(ThreadControlState.None);
    }


    /// <summary>
    /// Thread base class with public methods for controlling the thread.
    /// </summary>
    public class PublicThreadBase : ThreadBase
    {
        public virtual void Start()
        {
            StartThread();
        }
        public virtual bool Stop(bool wait = true)
        {
            return StopThread(wait);
        }
        public virtual bool Suspend(bool wait = true)
        {
            return SuspendThread(wait);
        }
        public virtual bool Resume(bool wait = true)
        {
            return ResumeThread(wait);
        }
    }
}
