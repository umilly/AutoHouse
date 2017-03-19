using Facade;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using ViewModelBase.Annotations;

namespace VMBase
{
    public class CustomViewBase<T> : UserControl,INotifyPropertyChanged, IView where T : IViewModel
    {
        protected ViewService _viewService;
        private T _viewModel;

        public CustomViewBase(ViewService viewService)
        {
            _viewService = viewService;
        }

        public CustomViewBase()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            var prop = ((expression.Body) as MemberExpression).Member.Name;
            OnPropertyChanged(prop);
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public T ViewModel
        {
            get { return _viewModel; }
            set
            {
                if(Object.Equals(value,_viewModel))
                    return;
                _viewModel = value;
                OnVMSet();
                OnPropertyChanged();
            }
        }

        IViewModel IView.ViewModel
        {
            get { return ViewModel; }
            set
            {
                ViewModel = (T)value; 
            }
        }

        public Type VmType
        {
            get { return typeof(T); }
        }

        public virtual void OnClose()
        {
            _viewService.CloseView(this);
        }

        public virtual void OnVMSet()
        {
            
        }
    }
}
