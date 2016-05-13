using Facade;
using System;
using System.Windows;
using Common;
using ViewModelBase;

namespace VMBase
{
    public class ViewService:ServiceBase,IViewService
    {
        
        public ViewService()
        {
            
        }

        public T CreateView<T>(int id=-1) where T : IView
        {
            return (T)CreateView(typeof (T),id); 
        }

        private IView CreateView(Type type, IViewModel viewModel)
        {
            var res = Activator.CreateInstance(type, this) as IView;
            res.ViewModel = viewModel;
            return res;
        }

        public T CreateView<T>(IViewModel viewModel)  where  T:IView
        {
            return (T)CreateView(typeof (T), viewModel) ;
        }

        public IView CreateView(Type type,int Id=-1) 
        {
            var res = Activator.CreateInstance(type, this) as IView;
            res.ViewModel = Use<IPool>().GetOrCreateVM(res.VmType,Id);
            return res;
        }
        public IView NextView { get; set; }
        public void ShowMessage(string res)
        {
            MessageBox.Show(res);
        }
    }


}