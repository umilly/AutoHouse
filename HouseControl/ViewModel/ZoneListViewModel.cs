using System.Collections.Generic;
using System.Linq;
using Facade;
using ViewModelBase;

namespace ViewModel
{
    public class ZoneListViewModel : ViewModelBase.ViewModelBase
    {
        public ZoneListViewModel(IServiceContainer container) : base(container)
        {
        }

        public void CreateZone()
        {
            var model=Use<IPool>().CreateDBObject<ZoneViewModel>();
            model.Name = "Новая зона";
            //Use<IPool>().SaveDB(TODO);
            OnPropertyChanged(()=>Zones);
        }
        public override int ID { get; set; }
        public IEnumerable<ZoneViewModel> Zones => Use<IPool>().GetViewModels<ZoneViewModel>().Except(new[]{Use<IPool>().GetViewModels<ZoneViewModel>().First()});
    }
}