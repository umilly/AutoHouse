using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ReactionService : ServiceBase, IReactionService
    {
        private readonly Queue<IViewModel> _checkQueue =new Queue<IViewModel>();
        public ReactionService() : base()
        {
            var timerThread = new Thread(CheckTasks);
            timerThread.Start();
        }

        private void CheckTasks()
        {
            while (true)
            {
                if(IsDisposed)
                    return;
                IViewModel _checkTask = null;
                lock (_checkQueue)
                {
                    if (_checkQueue.Any())
                        _checkTask = _checkQueue.Dequeue();
                }
                if (_checkTask == null)
                {
                    Thread.Sleep(1);
                    continue;
                }

                try
                {
                    CheckInternal(new[] {_checkTask});
                }
                catch (Exception e)
                {
                    Use<ILog>().LogNetException(e,"reaction check error");
                }
            }
        }

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

        public void Check(params IViewModel[] parametersViewModel)
        {
            lock (_checkQueue)
            {
               parametersViewModel.ForEach(a=> _checkQueue.Enqueue(a));
            }
            //return Task.Run(() => CheckInternal(parametersViewModel));
        }

        private void CheckInternal(IViewModel[] parametersViewModel)
        {
            List<ReactionViewModel> reacts = new List<ReactionViewModel>();
            foreach (var parameterViewModel in parametersViewModel)
            {
                if (parameterViewModel is ConditionViewModel condition)
                {
                    reacts.Add(condition.ParentReacton);
                }

                if (parameterViewModel is ParameterViewModel model)
                {
                    var paramReacts = ((IEntityObjectVM<Parameter>) model).Model.ComandParameterLinks
                        .Select(a => Use<IPool>().GetOrCreateDBVM<ReactionViewModel>(a.Command.Reaction)).Union(
                            Use<IPool>().GetViewModels<ConditionViewModel>()
                                .Where(a => a.Parameter1 == parameterViewModel || a.Parameter2 == parameterViewModel)
                                .Select(a => a.ParentReacton));
                    reacts.AddRange(paramReacts);
                }

                if (parameterViewModel is IEntityObjectVM<Sensor> sensor)
                {
                    var sensorReacts = sensor.Model.Conditions.Select(c =>
                        Use<IPool>().GetOrCreateDBVM<ConditionViewModel>(c).ParentReacton);
                    reacts.AddRange(sensorReacts);
                }

                if (parameterViewModel is ReactionViewModel react)
                {
                    reacts.Add(react);
                }
            }
            var mode = Use<IPool>().GetViewModels<ModeViewModel>().SingleOrDefault(a => a.IsSelected);

            reacts.Where(a=>a.Scenario.Parent==mode).Distinct().ForEach(a => a.Check());
        }
    }
}