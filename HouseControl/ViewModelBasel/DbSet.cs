using System.Collections.Generic;
using Facade;

namespace ViewModelBase
{
    internal class DbSet<TModel> : IDbSet<TModel> where TModel : class, IHaveID
    {
        public readonly Dictionary<long, IEntityObjectVM<TModel>> _setById =
            new Dictionary<long, IEntityObjectVM<TModel>>();

        public readonly Dictionary<IHaveID, IEntityObjectVM<TModel>> _setByModel =
            new Dictionary<IHaveID, IEntityObjectVM<TModel>>();

        public void Add(IEntityObjectVM<TModel> vm)
        {
            lock (this)
            {
                _setByModel[vm.Model] = vm;
                if (vm.Model.ID != 0)
                {
                    _setById[vm.Model.ID] = vm;
                }
            }
        }

        public void AddRange(IEnumerable<IEntityObjectVM<TModel>> vms)
        {
            lock (this)
            {
                vms.ForEach(
                    vm =>
                    {
                        _setByModel[vm.Model] = vm;
                        if (vm.Model.ID != 0)
                        {
                            _setById[vm.Model.ID] = vm;
                        }
                    });
            }
        }

        public IEnumerable<IViewModel> All
        {
            get
            {
                lock (this)
                {
                    return _setByModel.Values;
                }
            }
        }

        public void Clear()
        {
            lock (this)
            {
                _setById.Clear();
                _setByModel.Clear();
            }
        }

        public IEntityObjectVM Get(TModel model)
        {
            return _setByModel.ContainsKey(model) ? _setByModel[model] : null;
        }

        public IEntityObjectVM Get(long id)
        {
            return id == 0 || !_setById.ContainsKey(id) ? null : _setById[id];
        }

        public void Remove(TModel model)
        {
            lock (this)
            {
                if (_setByModel.ContainsKey(model))
                {
                    _setByModel.Remove(model);
                    _setById.Remove(model.ID);
                }
            }
        }

        public void Remove(long id)
        {
            lock (this)
            {
                if (_setById.ContainsKey(id))
                {
                    _setByModel.Remove(_setById[id].Model);
                    _setById.Remove(id);
                }
            }
        }
    }
}