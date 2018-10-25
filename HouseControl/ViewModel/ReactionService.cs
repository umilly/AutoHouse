using System.Collections.Generic;
using System.Linq;
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

        public void Check(params IViewModel[] parametersViewModel)
        {
            List<Reaction> reacts = new List<Reaction>();
            foreach (var parameterViewModel in parametersViewModel)
            {

                if (parameterViewModel is ConditionViewModel condition)
                {
                    reacts.Add((condition.ParentReacton as IEntityObjectVM<Reaction>).Model);
                }
                if (parameterViewModel is ParameterViewModel model)
                {
                    var paramReacts = ((IEntityObjectVM<Parameter>) model).Model.ComandParameterLinks
                        .Select(a => a.Command.Reaction).Union(
                            Use<IPool>().GetViewModels<ConditionViewModel>()
                                .Where(a => a.Parameter1 == parameterViewModel || a.Parameter2 == parameterViewModel)
                                .Select(a => ((IEntityObjectVM<Reaction>) a.ParentReacton).Model)).Distinct();
                    reacts.AddRange(paramReacts);
                }
                if (parameterViewModel is IEntityObjectVM<Sensor> sensor)
                {
                    var sensorReacts = sensor.Model.Conditions
                        .Select(a => a.Reaction);
                    reacts.AddRange(sensorReacts);
                }
                if (parameterViewModel is IEntityObjectVM<Reaction> react)
                {
                    reacts.Add(react.Model);
                }
            }
            reacts.Distinct().ForEach(a => Use<IPool>().GetOrCreateDBVM<ReactionViewModel>(a).Check());
        }
    }
}