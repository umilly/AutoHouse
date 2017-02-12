using System;
using System.Collections.Generic;
using System.Linq;
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
                    next = Use<IPool>().GetViewModels<ClientParameterViewModel>().FirstOrDefault(a => a.ID == next.NextParam); ;
                }
                return res;
            }
        }
        public ParameterProxy Param { get; set; }
        public override int ID
        {
            get { return Param.ID; }
            set {  }
        }

        public string Value => Param.Value;
        public ParameterTypeValue Type => Param.ParamType;
        public int CategoryId => Param.Category.HasValue? (int)Param.Category.Value:-1;
        public string CategoryName => Param.Category?.ToString() ?? "Не назначено";
        public int ZontId => Param.Sensor?.Zone.ID?? -1;
        public string ZoneName => Param.Sensor?.Zone.Name ?? "Не назначено";
        public string Name => Param.Name;
        public int NextParam => Param.NextParam;
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
            }
        }

        public bool TogggleValue { get; set; }
        public bool TogggleValue1
        {
            get
            {
                return false;
                //Param.ActualValue=="1"; 
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