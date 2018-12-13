using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using Facade;
using Model;
using ViewModel;
using ViewModelBase;

namespace ViewModel
{
    public class ParameterSetCommandVm : LinkedObjectVm<ParametrSetCommand>
    {
        public ParameterSetCommandVm(IServiceContainer container,  ParametrSetCommand model)
            : base(container,  model)
        {
        }

        public override int Color { get; } = 4;
        public override Type ParentType { get; }

        public override void LinklToParent(ITreeNode newParent)
        {
            if (!(newParent is ReactionViewModel))
                throw new InvalidEnumArgumentException("comand's parent must ve scenario");
            (newParent as ReactionViewModel).LinkChildParamCommand(Model);
        }
        public override ITreeNode Parent => Use<IPool>().GetOrCreateDBVM<ReactionViewModel>(Model.Reaction);
        public override IEnumerable<ITreeNode> Children { get; }
        public override string Value { get; set; }
        public ReactionViewModel Reaction { get { return Parent as ReactionViewModel;} }
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
                return Model.DestParameter == null ? null : Use<IPool>().GetOrCreateDBVM<ParameterViewModel>(Model.DestParameter);
            }
            set
            {
                value.LinkDestSetParam(Model);
                OnPropertyChanged();
                OnPropertyChanged(()=>InvertAvail);
            }
        }

        public ParameterViewModel SrcParameter1
        {
            get
            {
                return Model.SrcParameter1 == null ? null : Use<IPool>().GetOrCreateDBVM<ParameterViewModel>(Model.SrcParameter1);
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
                return Model.SrcParameter2 == null ? null : Use<IPool>().GetOrCreateDBVM<ParameterViewModel>(Model.SrcParameter2);
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

        public ISensorVM Sensor
        {
            get { return Model.Sensor == null ? null : Use<IPool>().GetOrCreateDBVM<ISensorVM>(Model.Sensor); }
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
        public IEnumerable<ISensorVM> AllSensors => Use<IPool>().GetViewModels<ISensorVM>()
            .Where(a=>a.Zone.IsGlobal||(Reaction!=null&& Reaction.Scenario.HaveZone(a.Zone)));

        public bool Invert
        {
            get {return Model.Invert; }
            set
            {
                Model.Invert = value;
                OnPropertyChanged();
            }
        }
        public bool InvertAvail
        {
            get { return Model.DestParameter!=null&&Model.DestParameter.ParameterType.ID== (int)ParameterTypeValue.Bool; }
        }
        public void Execute()
        {
            if (Cooldown==0)
            {
                SetValue();
            }
            else
            {
                Use<ITimerSerivce>().UnSubsctibe(this);
                Use<IUpdateTimer>().Subscribe(this, UpdateStatusMs, 100, true);
                Use<ITimerSerivce>().Subscribe(this,SetValue,Cooldown*1000);
            }
        }

        private void SetValue()
        {
            DestParameter.Value = SrcParameter1 != null ? SrcParameter1.Value : Sensor?.Value;
            if (InvertAvail && Invert)
            {
                DestParameter.Value = DestParameter.Value == "1" ? "0" : "1";
            }
        }

        public void LinkTo(Reaction reaction)
        {
            Model.Reaction = reaction;
        }
    }
}