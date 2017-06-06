using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ConditionViewModel : LinkedObjectVM<Condition>
    {
        private readonly CommandHandler _addCondition;

        public ConditionViewModel(IServiceContainer container, Models dataBase, Condition model)
            : base(container, dataBase, model)
        {
            if (IsFake)
                return;
            _addCondition = new CommandHandler(b => AddChildCondition());
            _contextMenu.Add(new CustomContextMenuItem("Ќовое подусловие", _addCondition));
            UpdateConextMenu();
        }

        public ReactionViewModel Reaction
            => Model.Reaction!=null ? Use<IPool>().GetDBVM<ReactionViewModel>(Model.Reaction) : null;

        public override ITreeNode Parent => Reaction ?? ParentCondition;
        public ITreeNode ParentCondition => Use<IPool>().GetDBVM<ConditionViewModel>(Model.ParentCondition);

        public override IEnumerable<ITreeNode> Children => Use<IPool>().GetViewModels<ConditionViewModel>().Where(a=>a.Parent==this);

        public override string Value
        {
            get { return _isComplete.ToString(); }
            set
            {
            }
        }

        public override bool? IsConnected { get { return _isComplete; }set {} }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(Model.Name) && Reaction != null;
        }

        public ParameterViewModel Parameter1 => Use<IPool>().GetDBVM<ParameterViewModel>(Model.Parameter1);
        public ParameterViewModel Parameter2 => Use<IPool>().GetDBVM<ParameterViewModel>(Model.Parameter2);
        public SensorViewModel Sensor => Use<IPool>().GetDBVM<SensorViewModel>(Model.Sensor);

        public IEnumerable<ConditionTypeViewModel> CondtionTypes
        {
            get
            {
                if (Children.OfType<ConditionViewModel>().Any())
                {
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int) ConditionTypeValue.And);
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.Or);
                    yield break;
                }
                if (Parameter1!=null||Parameter2!=null||Sensor!=null)
                {

                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.Equal);
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.NotEqual);
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.More);
                    yield return Use<IPool>().GetDBVM<ConditionTypeViewModel>((int)ConditionTypeValue.Less);
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
            get { return Use<IPool>().GetDBVM<ConditionTypeViewModel>(Model.ConditionType); }
            set
            {
                value.LinkCondtioin(Model);
                UpdateConextMenu();
                OnPropertyChanged();
                OnPropertyChanged(()=> PramsEnabled);
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
                return Use<IPool>().GetViewModels<SensorViewModel>()
                    .Where(a => a.Zone != null&& a.Zone.IsGlobal|| CurrentScenario.HaveZone(a.Zone))
                    .Cast<IConditionSource>()
                    .Union(Use<IPool>().GetViewModels<ParameterViewModel>())
                    .Union(new[] { EmptyValue.Instance});
            }
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
            get { return Parameter1 ?? (IConditionSource)Sensor?? EmptyValue.Instance; }
            set
            {
                
                 Model.Parameter1 = null;
                 Model.Sensor = null;
                (value as SensorViewModel)?.LinkCondition(Model);
                (value as ParameterViewModel)?.LinkCondition(Model,true);
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
            }
        }

        private void AddChildCondition()
        {
            var condition = Use<IPool>().CreateDBObject<ConditionViewModel>();
            condition.Model.ParentCondition = Model;
            condition.Name = "”словие";
            condition.CondtionType = Use<IPool>().GetViewModels<ConditionTypeViewModel>().First();
            OnPropertyChanged(() => Children);
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
                    return _isComplete;
                _isComplete = newVal;
            }
            catch (Exception ex)
            {
                Use<ILog>().Log(LogCategory.Configuration, $"Condition {Name} CheckFail: \r\n {ex.Message}" );
                _isComplete = false;
            }
            OnPropertyChanged(() => Value);
            OnPropertyChanged(() => IsConnected);
            return _isComplete;
        }

        private bool TypedCompare()
        {
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
                    var t1 = DateTime.Parse(s1.Value, CultureInfo.InvariantCulture.NumberFormat);
                    var t2 = DateTime.Parse(s2.Value, CultureInfo.InvariantCulture.NumberFormat);
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
                throw new ArgumentException($"compare v1:'{s1.Value}',v2:'{s2.Value}'",ex);
            }
            
        }

        public void LinkTo(Reaction model)
        {
            Model.Reaction = model;
        }
    }

    public class EmptyValue : IConditionSource
    {
        public static EmptyValue Instance { get; } = new EmptyValue();

        public string SourceName => "Ќе установлен";

        public string Value
        {
            get
            {
                throw new ArgumentOutOfRangeException("Ќастройка реакций не завершена, где то не установлено значение");
            }
        }

        public Type ValueType => typeof (object);
    }
}