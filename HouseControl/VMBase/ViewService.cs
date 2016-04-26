using Facade;
using System;
using System.Windows;
using ViewModelBase;

namespace VMBase
{
    public class ViewService:ServiceBase,IViewService
    {
        
        public ViewService()
        {
            
        }

        public T CreateView<T>(int id) where T : IView
        {
            return (T)CreateView(typeof (T),id); 
        }
        public IView CreateView(Type type,int Id) 
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