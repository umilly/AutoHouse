using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Common;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class MainViewModel : ViewModelBase.EmptyEntityVM
    {
        public MainViewModel(IServiceContainer container) : base(container)
        {
        }

        protected override async Task OnCreate()
        {

            var controllers = await Context.CustomQuery<Controller>(controller => controller.Include(a => a.Conditions).ToList());
            Use<IPool>().PrepareModels<ControllerVM, Controller>(controllers);

            var sensors = await Context.CustomQuery<Sensor>(sensor => sensor.Include(s=>s.Conditions).ToList());
            Use<IPool>().PrepareModels<SecondTypeSensor, Sensor>(sensors.Where(a=>a is CustomSensor));
            Use<IPool>().PrepareModels<FirstTypeSensor, Sensor>(sensors.Where(a => !(a is CustomSensor)));
            var zones = await Context.QueryModels<Zone>(zone => true);
            Use<IPool>().PrepareModels<ZoneViewModel, Zone>(zones);
            var ptypes = await Context.QueryModels<ParameterType>(pt => true);
            Use<IPool>().PrepareModels<ParameterTypeViewModel, ParameterType>(ptypes);
            var devices = await Context.QueryModels<CustomDevice>(pt => true);
            Use<IPool>().PrepareModels<CustomDeviceViewModel, CustomDevice>(devices);
            //params
            var paramss = await Context.QueryModels<Parameter>(pt => true);
            Use<IPool>().PrepareModels<ParameterViewModel, Parameter>(paramss);
            var paramscat = await Context.QueryModels<ParameterCategory>(pt => true);
            Use<IPool>().PrepareModels<ParameterCategoryVm, ParameterCategory>(paramscat);

            //reactions
            var modes = await Context.QueryModels<Mode>(pt => true);
            Use<IPool>().PrepareModels<ModeViewModel, Mode>(modes);
            var scenarios = await Context.QueryModels<Scenario>(pt => true);
            Use<IPool>().PrepareModels<ScenarioViewModel, Scenario>(scenarios);
            var reactions = await Context.CustomQuery<Reaction>(r => r.Include(a=>a.Scenario).ToList());
            Use<IPool>().PrepareModels<ReactionViewModel, Reaction>(reactions);
            var conditions = await Context.QueryModels<Condition>(pt => true);
            Use<IPool>().PrepareModels<ConditionViewModel, Condition>(conditions);
            var commands = await Context.QueryModels<Command>(pt => true);
            Use<IPool>().PrepareModels<CommandViewModel, Command>(commands);
            var condTypes = await Context.QueryModels<ConditionType>(pt => true);
            Use<IPool>().PrepareModels<ConditionTypeViewModel, ConditionType>(condTypes);
            var commandParams = await Context.QueryModels<ParametrSetCommand>(pt => true);
            Use<IPool>().PrepareModels<ParameterSetCommandVm, ParametrSetCommand>(commandParams);
            var sensorTypes = await Context.QueryModels<SensorType>(pt => true);
            Use<IPool>().PrepareModels<SensorTypeViewModel, SensorType>(sensorTypes);
            var templates= await Context.QueryModels<Template>(pt => true);
            Use<IPool>().PrepareModels<TemplateViewModel, Template>(templates);


            Use<IWebServer>().Start();
            Use<IReactionService>().Check();

            Use<ITimerSerivce>().Subscribe(this, UpdateControllers, 500, true);
            Use<IPool>().GetViewModels<FirstTypeSensor>().ForEach(a => a.UpdateValue());

        }

        private void UpdateControllers()
        {
            Use<IPool>().GetViewModels<ControllerVM>().ForEach(a=>a.Update());
            Use<IPool>().GetViewModels<FirstTypeSensor>().ForEach(a => a.UpdateValue());
            //Use<IReactionService>().Check();
        }

        

        public override int ID
        {
            get { return 1; }
            set { }
        }
        
        public void InitSettings()
        {

        }

        public void SaveSettings()
        {
            Use<IPool>().SaveDB(true);
        }
    }

    public class UpdateTimer : TimerService,IUpdateTimer
    {
        protected override void OnLoop()
        {
            base.OnLoop();
        }
    }
}
