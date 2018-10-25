using System;
using System.Collections.Generic;
using System.Linq;
using Facade;
using Model;

namespace ViewModelBase
{
    public abstract class LinkedObjectVm<T> : EntityObjectVm<T>, ITreeNode where T : class, IHaveID
    {
        protected readonly List<IContexMenuItem> _contextMenu;

        protected LinkedObjectVm(IServiceContainer container,  T model) : base(container,  model)
        {
            _contextMenu=new List<IContexMenuItem>();
            if (IsFake)
                return;
            _contextMenu.Add(new CustomContextMenuItem("Удалить", new CommandHandler(Delete)));
            _contextMenu.Add(new CustomContextMenuItem("Копировать", new CommandHandler(CopyTo)));
            _contextMenu.Add(new CustomContextMenuItem("Вставить", new PasteCommandHandler(PasteTo,Use<ICopyService>(),this)));

            Use<ITimerSerivce>().Subscribe(this, UpdateStatusMs, 100, true);
        }

        public virtual void OnChildrenChanded()
        {
            OnPropertyChanged(nameof(Children));
            Parent?.OnChildrenChanded();
        }
        public override void AddedToPool()
        {
            OnChildrenChanded();
        }
        private void UpdateStatusMs()
        {
            OnPropertyChanged(() => LastUpdateMs);
        }

        public virtual void OnChildrenChanged()
        {
            OnPropertyChanged(nameof(Children));
        }
        private void PasteTo(bool obj)
        {
            Use<ICopyService>().PasteTo(this);
        }

        private void CopyTo(bool obj)
        {
            Use<ICopyService>().SetCopyObject(this);
        }

        private void Delete(bool obj)
        {
            Delete();
        }

        public virtual ITreeNode Copy()
        {
            var res=Use<IPool>().CreateDBObject(GetType()) as ITreeNode;
            var newModel = (res as LinkedObjectVm<T>).Model;
            foreach (var property in newModel.GetType().GetProperties())
            {
                if(property.GetSetMethod()==null|| property.PropertyType.IsGenericType)
                    continue;
                var val = property.GetValue(Model);
                property.SetValue(newModel,val,null);
            }
            if (Children == null)
                return res;
            var list = Children.ToList();
            foreach (var treeNode in list)
            {
                var vm = treeNode.Copy();
                vm.LinklToParent(res);
            }
            return res;
        }

        public abstract void LinklToParent(ITreeNode Parent);
        public abstract Type ParentType { get; }
        public abstract ITreeNode Parent { get; }
        public abstract IEnumerable<ITreeNode> Children { get; }
        public abstract string Name { get;  set; }
        public abstract string Value { get; set; }
        public abstract bool? IsConnected { get; set; }
        public virtual int LastUpdateMs
        {
            get
            {
                if (IsConnected == null)
                    return 0;
                var now = DateTime.Now;
                var res=(int) (now - _lastUpdate).TotalMilliseconds;
                res = Math.Max(res, 1);
                if (!IsConnected.Value)
                    res = -res;
                return res;
            }
        }

        private DateTime _lastUpdate = DateTime.Now;

        protected void UpdateStatus()
        {
            _lastUpdate=DateTime.Now;
            OnPropertyChanged(()=>LastUpdateMs);
        }
        public List<IContexMenuItem> ContextMenu => _contextMenu;

        public virtual void OnChildDelete(ITreeNode node)
        {
            OnPropertyChanged(()=>Children);
        }

        public override void Delete()
        {
            var p = Parent;
            Children?.OfType<IEntityObjectVM>().ForEach(a => a.Delete());
            base.Delete();
            p?.OnChildDelete(this);
        }
    }
}