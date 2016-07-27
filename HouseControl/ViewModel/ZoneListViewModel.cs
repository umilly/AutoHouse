using System.Collections.Generic;
using Facade;
using ViewModelBase;

namespace ViewModel
{
    public class ZoneListViewModel : ViewModelBase.ViewModelBase
    {
        public ZoneListViewModel(IServiceContainer container) : base(container)
        {
        }

        public override int ID { get; set; }
        public IEnumerable<ZoneViewModel> Zones => Use<IPool>().GetViewModels<ZoneViewModel>();
    }
}