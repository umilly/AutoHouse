using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ConditionViewModel : LinkedObjectVm<Condition>,IConditionParent
    {
        private readonly CommandHandler _addCondition;
        public override int Color { get; } = 2;

        public ConditionViewModel(IServiceContainer container, Condition model)
            : base(container,model)
        {
            if (IsFake)
                return;
            _addCondition = new CommandHandler(b => AddChildCondition());
            _contextMenu.Add(new CustomContextMenuItem("Ќовое подусловие", _addCondition));
            UpdateConextMenu();
        }
        
        public ReactionViewModel Reaction
            => Model.Reaction!=null ? Use<IPool>().GetOrCreateDBVM<ReactionViewModel>(Model.Reaction) : null;

        public override void LinklToParent(ITreeNode newParent)
        {
            if (newParent is ConditionViewModel pc)
            {
                Model.ParentCondition = pc.Model;
                Model.Reaction = null;
                OnPropertyChanged(() => Children);
                pc.OnChildrenChanded();
            }
            if (newParent is ReactionViewModel)
            {
               (newParent as ReactionViewModel).LinkCondition(Model);
            }
            Use<IReactionService>().Check(this);
        }

        public override ITreeNode Parent => Reaction ?? ParentCondition;
        public override Type ParentType { get { return typeof(IConditionParent); } }
        public ITreeNode ParentCondition => Use<IPool>().GetOrCreateDBVM<ConditionViewModel>(Model.ParentCondition);

        public override IEnumerable<ITreeNode> Children => Use<IPool>().GetViewModels<ConditionViewModel>().Where(a=>a.Parent==this);

        public override string Value
        {
            get { return _isComplete.ToString(); }
            set
            {
            }
        }

        public override VMState VMState { get { return _isComplete?VMState.Positive : VMState.Negative; }}

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name) && Reaction != null;
        }

        public ParameterViewModel Parameter1 => Use<IPool>().GetOrCreateDBVM<ParameterViewModel>(Model.Parameter1);
        public ParameterViewModel Parameter2 => Use<IPool>().GetOrCreateDBVM<ParameterViewModel>(Model.Parameter2);
        public ISensorVM Sensor => Use<IPool>().GetOrCreateDBVM<ISensorVM>(Model.Sensor);
        public ControllerVM Controller => Use<IPool>().GetOrCreateDBVM<ControllerVM>(Model.Controller);

        public IEnumerable<ConditionTypeViewModel> CondtionTypes
        {
            get
            {
                if (Children.OfType<ConditionViewModel>().Any())
                {
                    yield return Use<IPool>().GetViewModel<ConditionTypeViewModel>((int) ConditionTypeValue.And);
                    yield return Use<IPool>().GetViewModel<ConditionTypeViewModel>((int)ConditionTypeValue.Or);
                    yield break;
                }
                if (Parameter1!=null||Parameter2!=null||Sensor!=null)
                {

                    yield return Use<IPool>().GetViewModel<ConditionTypeViewModel>((int)ConditionTypeValue.Equal);
                    yield return Use<IPool>().GetViewModel<ConditionTypeViewModel>((int)ConditionTypeValue.NotEqual);
                    yield return Use<IPool>().GetViewModel<ConditionTypeViewModel>((int)ConditionTypeValue.More);
                    yield return Use<IPool>().GetViewModel<ConditionTypeViewModel>((int)ConditionTypeValue.Less);
                    yield break;
                }
                foreach (var ctype in Use<IPool>().GetViewModels<ConditionTypeViewModel>())
                {
                    yield return ctype;
                }

            }
        }

        public ConditionTypeViewModel CondtionType
        {
            get { return Use<IPool>().GetOrCreateDBVM<ConditionTypeViewModel>(Model.ConditionType); }
            set
            {
                value.LinkCondtioin(Model);
                UpdateConextMenu();
                OnPropertyChanged();
                OnPropertyChanged(()=> PramsEnabled);
                Use<IReactionService>().Check(this);
            }
        }

        private void UpdateConextMenu()
        {
            _addCondition.Executable = !PramsEnabled;
        }

        public bool PramsEnabled => CondtionType != null
                                    && (CondtionType.ID != (int)ConditionTypeValue.And
                                    && CondtionType.ID != (int)ConditionTypeValue.Or);

        public IEnumerable<IConditionSource> LeftParamSource
        {
            get
            {
                var res=
                Use<IPool>().GetViewModels<ISensorVM>()
                    .Where(a => a.Zone != null&& a.Zone.IsGlobal|| CurrentScenario.HaveZone(a.Zone))
                    .Cast<IConditionSource>()
                    .Union(Use<IPool>().GetViewModels<ParameterViewModel>())
                    .Union(Use<IPool>().GetViewModels<ControllerVM>())
                    .Union(new[] { EmptyValue.Instance}).ToList();
                return res;
            }
        }

        public ReactionViewModel ParentReacton
        {
            get => Reaction ?? (ParentCondition as ConditionViewModel).ParentReacton;
        }

        public ScenarioViewModel CurrentScenario
        {
            get
            {
                if (Reaction != null)
                    return Reaction.Scenario;
                if(ParentCondition==null)
                    throw new ArgumentOutOfRangeException("CurrentScenario", "условие не прив€зано ни к родительскому условию ни к реакции");
                return (ParentCondition as ConditionViewModel).CurrentScenario;
            }
        }

        public IEnumerable<IConditionSource> RightParamSource
        {
            get
            {
                return Use<IPool>().GetViewModels<ParameterViewModel>()
                    .Cast<IConditionSource>()
                    .Union(new[] { EmptyValue.Instance });
            }
        }

        public IConditionSource LeftParam
        {
            get { return Parameter1 ?? (IConditionSource)Sensor?? (IConditionSource)Controller ?? EmptyValue.Instance; }
            set
            {

                Model.Parameter1 = null;
                Model.Sensor = null;
                Model.Controller = null;
                (value as ISensorVM)?.LinkCondition(Model);
                (value as ParameterViewModel)?.LinkCondition(Model, true);
                (value as ControllerVM)?.LinkCondition(Model);
                OnPropertyChanged(nameof(CondtionTypes));
                Use<IReactionService>().Check(this);
            }
        }
        public IConditionSource RightParam
        {
            get { return Parameter2??(IConditionSource)EmptyValue.Instance; }
            set
            {
                if (value is EmptyValue)
                {
                    Model.Parameter2 = null;
                }
                (value as ParameterViewModel)?.LinkCondition(Model, false);
                OnPropertyChanged(nameof(CondtionTypes));
                Use<IReactionService>().Check(this);
            }
        }

        private void AddChildCondition()
        {
            var condition = Use<IPool>().CreateDBObject<ConditionViewModel>();
            condition.Model.ParentCondition = Model;
            condition.Name = "”словие";
            condition.CondtionType = Use<IPool>().GetViewModels<ConditionTypeViewModel>().First();
            OnPropertyChanged(() => Children);
            Use<IReactionService>().Check(this);
        }

        public override string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        private bool _isComplete = false;

        public bool CheckComplete()
        {

            try
            {
                var newVal = TypedCompare();
                if (_isComplete == newVal)
                {
                    UpdateStatus();
                    return _isComplete;
                }
                _isComplete = newVal;
            }
            catch (Exception ex)
            {
                Use<ILog>().Log(LogCategory.Configuration, $"Condition {Name} CheckFail: \r\n" );
                Use<ILog>().Log(LogCategory.Configuration, ex);
                _isComplete = false;
            }
            OnPropertyChanged(() => Value);
            UpdateStatus();
            return _isComplete;
        }

        private bool TypedCompare()
        {
            if(CondtionType==null)
                throw new ArgumentException("“ип услови€ должен быть определЄн");
            switch (CondtionType.TypeValue)
            {
                case ConditionTypeValue.And:
                    return Children.Cast<ConditionViewModel>().All(a => a.CheckComplete());
                case ConditionTypeValue.Or:
                    return Children.Cast<ConditionViewModel>().Any(a => a.CheckComplete());
                case ConditionTypeValue.Less:
                    return Compare(LeftParam, RightParam) == -1;
                case ConditionTypeValue.More:
                    return Compare(LeftParam, RightParam) == 1;
                case ConditionTypeValue.Equal:
                    return Compare(LeftParam, RightParam) == 0;
                case ConditionTypeValue.NotEqual:
                    return Compare(LeftParam, RightParam) != 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override ITreeNode Copy()
        {
            var res= base.Copy();
            return res;
        }

        public int Compare(IConditionSource s1, IConditionSource s2)
        {
            try
            {
                if (s1.ValueType == typeof(DateTime))
                {
                    if (s2.ValueType != typeof(DateTime))
                    {
                        throw new ArgumentOutOfRangeException(
                            "типы значений не совпадают, врем€ можно сравнить только со временем");
                    }
                    DateTime.TryParse(s1.Value, CultureInfo.InvariantCulture.NumberFormat, DateTimeStyles.AllowInnerWhite, out var t1);
                    DateTime.TryParse(s2.Value, CultureInfo.InvariantCulture.NumberFormat, DateTimeStyles.AllowInnerWhite, out var t2);
                    return t1.CompareTo(t2);
                }
                if (s1.ValueType == typeof(bool) || s1.ValueType == typeof(int) || s1.ValueType == typeof(float))
                {
                    if (s2.ValueType != typeof(bool) && s2.ValueType != typeof(int) && s2.ValueType != typeof(float))
                    {
                        throw new ArgumentOutOfRangeException("типы значений не сравнимы");
                    }
                    var f1 = float.Parse(s1.Value,CultureInfo.InvariantCulture.NumberFormat);
                    var f2 = float.Parse(s2.Value, CultureInfo.InvariantCulture.NumberFormat);
                    return f1.CompareTo(f2);
                }
                if (s1.ValueType == typeof(string))
                {
                    if (s2.ValueType != typeof(string))
                        throw new ArgumentOutOfRangeException("строки можно сравнивать только с строками");
                    return string.CompareOrdinal(s1.Value, s2.Value);
                }
                throw new ArgumentOutOfRangeException("сравнеение таких типов не определено");

            }
            catch (Exception ex)
            {
                throw new ArgumentException($"compare error \r\n condition={Name};reaction={ParentReacton} v1:'{s1.Value}',v2:'{s2.Value}'",ex);
            }
            
        }

        public void LinkTo(Reaction model)
        {
            Model.Reaction = model;
            Use<IReactionService>().Check(this);
        }
    }

    public interface IConditionParent
    {
    }
}