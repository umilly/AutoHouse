using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    public class ParameterSetCommandVm : LinkedObjectVM<ParametrSetCommand>
    {
        public ParameterSetCommandVm(IServiceContainer container, Models dataBase, ParametrSetCommand model)
            : base(container, dataBase, model)
        {
        }

        public override ITreeNode Parent => Use<IPool>().GetDBVM<ReactionViewModel>(Model.Reaction);
        public override IEnumerable<ITreeNode> Children { get; }
        public override string Value { get; set; }
        public override bool? IsConnected { get; set; }

        public override bool Validate()
        {
            return true;
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

        public ParameterViewModel DestParameter
        {
            get
            {
                return Model.DestParameter == null ? null : Use<IPool>().GetDBVM<ParameterViewModel>(Model.DestParameter);
            }
            set
            {
                value.LinkDestSetParam(Model);
                OnPropertyChanged();
            }
        }

        public ParameterViewModel SrcParameter1
        {
            get
            {
                return Model.SrcParameter1 == null ? null : Use<IPool>().GetDBVM<ParameterViewModel>(Model.SrcParameter1);
            }
            set
            {
                if (value == null)
                {
                    Model.SrcParameter1 = null;
                }
                else
                {
                    value.LinkSrcSetParam1(Model);
                    Sensor = null;
                }
                OnPropertyChanged();
            }
        }

        public ParameterViewModel SrcParameter2
        {
            get
            {
                return Model.SrcParameter2 == null ? null : Use<IPool>().GetDBVM<ParameterViewModel>(Model.SrcParameter2);
            }
            set
            {
                if (value == null)
                {
                    Model.SrcParameter2 = null;
                }
                else
                {
                    value.LinkSrcSetParam2(Model);
                }

                OnPropertyChanged();
            }
        }

        public SensorViewModel Sensor
        {
            get { return Model.Sensor == null ? null : Use<IPool>().GetDBVM<SensorViewModel>(Model.Sensor); }
            set
            {
                if (value == null)
                {
                    Model.Sensor = null;
                }
                else
                {
                    value.LinkSetComand(Model);
                    SrcParameter1 = null;
                }
                OnPropertyChanged();
            }
        }

        public int Cooldown
        {
            get { return Model.Cooldown; }
            set { Model.Cooldown = value; }
        }
        public string CooldownString
        {
            get { return Cooldown.ToString(); }
            set
            {
                int result;
                if (int.TryParse(value, out result))
                    Cooldown = result;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ParameterViewModel> AllParams => Use<IPool>().GetViewModels<ParameterViewModel>();
        public IEnumerable<SensorViewModel> AllSensors => Use<IPool>().GetViewModels<SensorViewModel>();
        private DateTime ExecuteTime=DateTime.MinValue;
        public void Execute()
        {
            var now = DateTime.Now;
            if ((now - ExecuteTime).TotalSeconds > Cooldown)
            {
                DestParameter.Value = SrcParameter1 != null ? SrcParameter1.Value : Sensor?.Value;
                ExecuteTime = now;
            }
        }

        public void LinkTo(Reaction reaction)
        {
            Model.Reaction = reaction;
        }
    }
}