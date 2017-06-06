using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ClientParameterViewModel:ViewModelBase.ViewModelBase
    {
        private bool? _isFirst;

        public ClientParameterViewModel(IServiceContainer container) : base(container)
        {
        }

        public List<ClientParameterViewModel> Chain
        {
            get
            {
                if (!IsFirst) return null;
                var res=new List<ClientParameterViewModel>();
                var next = this;
                while (next!=null)
                {
                    res.Add(next);
                    next = Use<IPool>().GetViewModels<ClientParameterViewModel>().FirstOrDefault(a => a.ID>=0&& a.ID == next.NextParam); ;
                }
                return res;
            }
        }

        public ParameterProxy Param
        {
            get { return _param; }
            set
            {
                _param = value;
                OnValueChanged();
            }
        }

        private void OnValueChanged()
        {
            OnPropertyChanged(() => TogggleValue);
            OnPropertyChanged(() => SliderValue);
            OnPropertyChanged(() => Chain);
            OnPropertyChanged(() => Name);
            OnPropertyChanged(() => ZoneName);
            OnPropertyChanged(() => CategoryName);
            OnPropertyChanged(() => Value);
            OnPropertyChanged(() => TogggleValue);
            OnPropertyChanged(() => SliderValue);
            OnPropertyChanged(() => NewValue);
        }

        public override int ID
        {
            get { return Param?.ID ?? -1; }
            set {  }
        }

        public string Value => Param.Value;
        public ParameterTypeValue Type => Param.ParamType;
        public int CategoryId => Param.Category!=null? (int)Param.Category.ID:-1;
        public string CategoryName => Param.Category?.Name ?? "Не назначено";
        public int ZontId => Param.Sensor?.Zone.ID?? -1;
        public string ZoneName => Param.Sensor?.Zone.Name ?? "Не назначено";
        public string Name => Param.Name;
        public int NextParam => Param?.NextParam??-1;
        public string Url
        {
            get
            {
                return
                    $"http://{Use<IPool>().GetViewModels<ClientOptionsViewModel>().Single().ServerIP}:{Use<IPool>().GetViewModels<ClientOptionsViewModel>().Single().ServerPort}/SetParam?{ID}?{NewValue}";
            }
        }

        public string NewValue
        {
            get { return _newValue; }
            set
            {
                _newValue = value;
                Use<INetworkService>().SyncRequest(Url);
                OnValueChanged();
            }
        }

        //public bool TogggleValue { get; set; }
        public bool TogggleValue
        {
            get
            {
                return Param.ActualValue=="1"; 
            }
            set
            {
                _togggleValue = value;
                NewValue = (value ? 1 : 0).ToString();
            }
        }

        public bool IsFirst
        {
            get
            {
                if (Param == null)
                    return false;
                if (!_isFirst.HasValue)
                {
                    _isFirst = Use<IPool>().GetViewModels<ClientParameterViewModel>().All(a => a.NextParam != ID);
                }
                return _isFirst.Value;
            }
            
        }

        public string ParamImage => "pack://application:,,,/Resources/water.png";//: "pack://application:,,,/Resources/radioOn.png";
        public bool ImageIsVisible => true;
        public bool ToggleIsVisible => Param.ParamType == ParameterTypeValue.Bool;
        
        public bool SliderIsVisible=> Param.ParamType == ParameterTypeValue.Int || Param.ParamType == ParameterTypeValue.Float;
        public int SliderMin => Param.Sensor?.MinValue??0;
        public int SliderMax => Param.Sensor?.MaxValue??0;
        public int _currentSliderValue = 0;
        private bool _togggleValue;
        private string _newValue;
        private ParameterProxy _param;

        public int SliderValue
        {
            get { return _currentSliderValue; }
            set
            {
                _currentSliderValue = value;
                NewValue = value.ToString();
            }
        }

        public ClientParameterViewModel Self
        {
            get
            {
                return this; 
                
            }
        }

        public void Delete()
        {
            Use<IPool>().RemoveVM(typeof(ClientParameterViewModel), ID);
        }
    }
}