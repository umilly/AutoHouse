using Facade;
using System;
using System.Windows.Controls;

namespace VMBase
{
    public class CustomViewBase<T> : UserControl, IView where T : IViewModel
    {
        protected ViewService _viewService;

        public CustomViewBase(ViewService viewService)
        {
            _viewService = viewService;
        }

        public CustomViewBase()
        {
        }

        public T ViewModel { get; set; }
        IViewModel IView.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (T)value; }
        }

        public Type VmType
        {
            get { return typeof(T); }
        }
    }
}
