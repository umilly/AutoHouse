using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using ViewModelBase;

namespace ViewModel
{
    public class ParameterFilterVM : ViewModelBase.ViewModelBase, IPublicParam
    {
        private Type _valueType;
        private string _value;
        private ParameterTypeViewModel _paramType;
        private ParameterCategoryVm _category;
        private SensorViewModel _sensor;
        private string _name;
        private string _description;
        private bool _isPublic;
        private IConditionSource _nextParam;
        public event Action OnFilterChanged;

        public ParameterFilterVM(IServiceContainer container) : base(container)
        {
        }
        public override int ID { get { return -1; }set{}}

        public Type ValueType
        {
            get { return _valueType; }
            set
            {
                _valueType = value;
                OnPropertyChanged();
                FilterChanded();
            }
        }

        private void FilterChanded()
        {
            if(!_blockEvent)
                OnFilterChanged?.Invoke();
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
                FilterChanded();
            }
        }

        public IEnumerable<ParameterTypeViewModel> ParamTypes => Use<IPool>().GetViewModels<ParameterTypeViewModel>();
        public IEnumerable<ParameterCategoryVm> Categories => Use<IPool>().GetViewModels<ParameterCategoryVm>();
        public IEnumerable<SensorViewModel> Sensors => Use<IPool>().GetViewModels<SensorViewModel>();

        public ParameterTypeViewModel ParamType
        {
            get { return _paramType; }
            set
            {
                _paramType = value;
                OnPropertyChanged();
                FilterChanded();
            }
        }

        public ParameterCategoryVm Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged();
                FilterChanded();
            }
        }

        public SensorViewModel Sensor
        {
            get { return _sensor; }
            set
            {
                _sensor = value;
                OnPropertyChanged();
                FilterChanded();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
                FilterChanded();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
                FilterChanded();
            }
        }

        public bool IsEditable => true;

        public bool IsPublic
        {
            get { return _isPublic; }
            set
            {
                _isPublic = value;
                OnPropertyChanged();
                FilterChanded();
            }
        }

        public IConditionSource NextParam
        {
            get { return _nextParam; }
            set
            {
                _nextParam = value;
                OnPropertyChanged();
                FilterChanded();
            }
        }

        public IEnumerable<IConditionSource> AllParams
        {
            get
            {
                return
                    Use<IPool>()
                        .GetViewModels<ParameterViewModel>()
                        .Cast<IConditionSource>()
                        .Union(new[] { EmptyValue.Instance});
            }
        }

        public string Ident => "Фильтр:";

        public bool IsMatch(ParameterViewModel vm)
        {
            bool isMatch = true;
            if (Category!=null)
                isMatch &= vm.Category== Category;
            if (ParamType != null)
                isMatch &= vm.ParamType == ParamType;
            if (NextParam != null)
                isMatch &= vm.NextParam == NextParam;
            if (ValueType != null)
                isMatch &= vm.ValueType == ValueType;
            if (!string.IsNullOrEmpty(Value))
                isMatch &= vm.Value == Value;
            if (!string.IsNullOrEmpty(Name))
                isMatch &= vm.Name.Contains(Name);
            if (!string.IsNullOrEmpty(Description))
                isMatch &= vm.Name.Contains(Description);
            return isMatch;
        }

        private bool _blockEvent = false;
        public void Clear()
        {
            try
            {
                _blockEvent = true;
                Category = null;
                ParamType = null;
                NextParam = null;
                ValueType = null;
                Value = String.Empty;
                Name = String.Empty;
                Description = String.Empty;
            }
            finally
            {
                _blockEvent = false;
            }
            FilterChanded();
        }
    }
}