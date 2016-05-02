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

        public T CreateView<T>(int id=0) where T : IView
        {
            return (T)CreateView(typeof (T),id); 
        }
        public IView CreateView(Type type,int Id=0) 
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