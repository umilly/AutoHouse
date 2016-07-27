using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Facade;

namespace ViewModel
{
    public class TimerService : ServiceBase,ITimerSerivce
    {
        private Thread _timerThread;
        
        public override void OnContainerSet()
        {
            base.OnContainerSet();
            _timerThread=new Thread(checkTasks);
            _timerThread.Start();
        }

        private void checkTasks(object state)
        {
            while (true)
            {
                var now = DateTime.Now;

                foreach (var timerTask in _tasks)
                {
                    var needInvoke = timerTask.Created.Add(new TimeSpan(0, 0, 0, 0, timerTask.PeriodMS)) < now;
                    if (needInvoke)
                    {
                        timerTask.Callback();
                        if (timerTask.Repeat)
                        {
                            timerTask.Created = now;
                        }
                        else
                        {
                            _tasks.Remove(timerTask);
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }

        public void Exit()
        {
            _timerThread.Abort();
        }
        private readonly List<TimerTask>  _tasks=new List<TimerTask>();
        public void Subsctibe(object key, Action action,int waitMilliSeconds,bool repeat=false)
        {
            _tasks.Add(new TimerTask(key,waitMilliSeconds, DateTime.Now, action, repeat));
        }
        public void UnSubsctibe(object key)
        {
            var toRemove = _tasks.Where(a=>a.Key==key).ToList();
            toRemove.ForEach(a=>_tasks.Remove(a));
        }
    }

    public class TimerTask
    {
        public object Key { get; set; }
        public int PeriodMS { get; set; }
        public DateTime Created { get; set; }
        public Action Callback { get; set; }
        public bool Repeat { get; set; }

        public TimerTask(object key, int periodMs, DateTime created, Action callback, bool repeat)
        {
            Key = key;
            PeriodMS = periodMs;
            Created = created;
            Callback = callback;
            Repeat = repeat;
        }
    }
}