using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
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

        public Task Check(params IViewModel[] parametersViewModel)
        {
            return Task.Run(() => CheckInternal(parametersViewModel));
            
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

            reacts.Distinct().ForEach(a => a.Check());
        }
    }
}