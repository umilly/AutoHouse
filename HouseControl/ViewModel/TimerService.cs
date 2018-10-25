using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Common;
using Facade;
using ViewModelBase;

namespace ViewModel
{
    public sealed class TimerService : ITimerSerivce
    {
        private readonly Queue<Action> _delegates = new Queue<Action>();

        private readonly HashSet<TimerTask> _tasks = new HashSet<TimerTask>();
        private readonly Thread _timerThread;
        private bool _closing;

        public TimerService()
        {
            _timerThread = new Thread(CheckTasks);
            _timerThread.Start();
        }

        public void Dispose()
        {
            Exit();
            _tasks.Clear();
            _delegates.Clear();
        }

        public void Reset()
        {
            lock (_tasks)
            {
                _tasks.Clear();
                _delegates.Clear();
            }
        }

        public void Subscribe(object key, Action action, int waitMilliSeconds, bool repeat = false)
        {
            lock (_tasks)
            {
                _tasks.Add(new TimerTask(key, waitMilliSeconds, DateTime.Now, action, repeat));
            }
        }

        public void UnSubsctibe(object key)
        {
            List<TimerTask> toRemove;
            lock (_tasks)
            {
                toRemove = _tasks.Where(a => a.Key == key).ToList();
            }

            toRemove.ForEach(
                task =>
                {
                    lock (task)
                    {
                        task.Dispose();
                    }

                    SafeRemoveTask(task);
                });
        }

        public void Exit()
        {
            _closing = true;
        }

        private Action AddItemToExecution(TimerTask executeTask)
        {
            executeTask.AddedToQueue();
            var res = new Action(
                () => { ProcessTask(executeTask); });
            return res;
        }

        private void CheckTasks(object state)
        {
            while (true)
            {
                if (_closing)
                {
                    return;
                }

                TimerTask executeTask = null;
                lock (_tasks)
                {
                    executeTask = _tasks.FirstOrDefault(a => a.State == TaskState.WaitsExecute);
                }

                if (executeTask != null)
                {
                    _delegates.Enqueue(AddItemToExecution(executeTask));
                    continue;
                }

                if (!_delegates.Any())
                {
                    Thread.Sleep(100);
                    continue;
                }

                _delegates.Dequeue()();
            }
        }

        private void ProcessTask(TimerTask executeTask)
        {
            lock (executeTask)
            {
                if (executeTask.State == TaskState.Disposed)
                {
                    SafeRemoveTask(executeTask);
                    return;
                }

                executeTask.Execute();
                if (executeTask.State == TaskState.Complete)
                {
                    SafeRemoveTask(executeTask);
                }
            }
        }

        private void SafeRemoveTask(TimerTask executeTask)
        {
            lock (_tasks)
            {
                if (_tasks.Contains(executeTask))
                {
                    _tasks.Remove(executeTask);
                }
            }
        }

        public void SetContainer(IServiceContainer container)
        {
            
        }
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, List<ISubscriberAction>> _subscribers =
            new Dictionary<Type, List<ISubscriberAction>>();

        public void Publish<T>(T e) where T : IEvent
        {
            var t = typeof(T);
            if (!_subscribers.ContainsKey(t))
            {
                return;
            }

            _subscribers[t].ForEach(a => a.Execute(e));
        }

        public void Subscribe<T>(object o, Action<T> action) where T : IEvent
        {
            var t = typeof(T);
            if (!_subscribers.ContainsKey(t))
            {
                _subscribers[t] = new List<ISubscriberAction>();
            }

            lock (_subscribers[t])
            {
                _subscribers[t].Add(new SubscriberAction<T>(o, action));
            }
        }

        public void UnSubscribe(object o)
        {
            foreach (var subscriberList in _subscribers)
            {
                lock (subscriberList.Value)
                {
                    var toRemove =
                        subscriberList.Value.Where(a => a.Subscriber == o || a.Subscriber == null).ToList();
                    toRemove.ForEach(a => subscriberList.Value.Remove(a));
                }
            }
        }
    }

    internal class SubscriberAction<T> : ISubscriberAction where T : IEvent
    {
        public SubscriberAction(object subscriber, Action<T> action)
        {
            _subscriber = new WeakReference<object>(subscriber);
            _action = action;
        }

        private Action<T> _action { get; }
        private WeakReference<object> _subscriber { get; }

        public void Execute(IEvent e)
        {
            _subscriber.TryGetTarget(out var target);
            if (target != null && _action != null)
            {
                _action((T)e);
            }
        }

        public object Subscriber
        {
            get
            {
                _subscriber.TryGetTarget(out var target);
                return target;
            }
        }
    }

    internal interface ISubscriberAction
    {
        object Subscriber { get; }
        void Execute(IEvent e);
    }

    public class Settings : ISettings
    {
        public Settings()
        {
            if (!File.Exists("Settings.ini"))
            {
                return;
            }
            var settings = File.ReadAllText("Settings.ini");
            foreach (var setting in settings.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (setting.ToLower().Contains("loglevel"))
                {
                    LogLevel = int.Parse(setting.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim());
                }
            }
        }

        public int LogLevel { get; set; }
        public void SetContainer(IServiceContainer container)
        {
            
        }
    }

    public class ReactionService : ServiceBase, IReactionService
    {
        public void Check()
        {
            var reacts=Use<IPool>().GetViewModels<ReactionViewModel>();
            var mode = Use<IPool>().GetViewModels<ModeViewModel>().SingleOrDefault(a => a.IsSelected);
            if (mode!=null)
            {
                reacts = reacts.Where(a => a.Scenario.Parent.ID == mode.ID);
                reacts.ForEach(a => a.Check());
            }
            
        }
    }

    public enum TaskState
    {
        WaitsTime,
        WaitsExecute,
        InQueue,
        Disposed,
        Executing,
        Complete
    }

}