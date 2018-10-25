using System;

namespace ViewModel
{
    public class TimerTask
    {
        private DateTime _created;

        private DateTime _executeTime;
        private bool _isAddedToQueue;
        private bool _isCompleted;

        private bool _isDisposed;
        private bool _isExecuting;

        public TimerTask(object key, int periodMs, DateTime created, Action callback, bool repeat)
        {
            Key = key;
            PeriodMS = periodMs;
            Created = created;
            Callback = callback;
            Repeat = repeat;
            if (Repeat)
            {
                Created = created - TimeSpan.FromMilliseconds(periodMs);
            }
        }

        public DateTime Created
        {
            get => _created;
            set
            {
                _created = value;
                _executeTime = _created.AddMilliseconds(PeriodMS);
            }
        }

        public object Key { get; set; }
        public int PeriodMS { get; set; }

        public TaskState State
        {
            get
            {
                if (_isDisposed)
                {
                    return TaskState.Disposed;
                }

                if (_isExecuting)
                {
                    return TaskState.Executing;
                }

                if (_isAddedToQueue)
                {
                    return TaskState.InQueue;
                }

                if (_isCompleted)
                {
                    return TaskState.Complete;
                }

                return
                    NeedInvoke ? TaskState.WaitsExecute : TaskState.WaitsTime;
            }
        }

        private Action Callback { get; set; }

        private bool NeedInvoke
        {
            get => DateTime.Now > _executeTime;
        }

        private bool Repeat { get; }

        public void AddedToQueue()
        {
            _isAddedToQueue = true;
        }

        public void Dispose()
        {
            if (_isExecuting)
            {
                throw new InvalidOperationException("cant dispose executing task");
            }

            _isDisposed = true;
            _isAddedToQueue = false;
            Callback = null;
        }

        public void Execute()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("cant execute disposed task");
            }

            _isExecuting = true;
            Callback();
            _isAddedToQueue = false;
            _isExecuting = false;
            if (!Repeat)
            {
                _isCompleted = true;
            }
            else
            {
                Created = DateTime.Now;
            }
        }
    }
}