using System.Collections.Generic;
using System.Linq;
using Facade;

namespace ViewModelBase
{
    internal class Set<TVm> : ISet<TVm> where TVm : class, IViewModel
    {
        public Dictionary<long, TVm> _setById = new Dictionary<long, TVm>();

        public void Add(TVm vm)
        {
            lock (this)
            {
                _setById[vm.ID] = vm;
            }
        }

        public IEnumerable<IViewModel> All
        {
            get
            {
                lock (this)
                {
                    return _setById.Values.ToList();
                }
            }
        }

        public IViewModel Find(long id)
        {
            if (_setById.ContainsKey(id))
            {
                return _setById[id];
            }
            return null;
        }

        public void Remove(long id)
        {
            if (_setById.ContainsKey(id))
            {
                _setById.Remove(id);
            }
        }

        public void AddRange(IEnumerable<TVm> vms)
        {
            lock (this)
            {
                foreach (var vm in vms)
                {
                    _setById[vm.ID] = vm;
                }
            }
        }
    }
}