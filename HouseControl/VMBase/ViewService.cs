using Facade;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly Dictionary<Type,Type> VmToViewMap=new Dictionary<Type, Type>();
        public void FillTypes(Type[] types)
        {
            foreach (var type in types)
            {
                if(type.IsAbstract)
                    continue;
                var baseType = type.BaseType;
                if (baseType.IsGenericType)
                    VmToViewMap[baseType.GetGenericArguments().First()] = type;
            }
        }
        public T CreateView<T>(int id=-1) where T : IView
        {
            return (T)CreateView(typeof (T),id); 
        }

        private IView CreateView(Type type, IViewModel viewModel)
        {
            var res = Activator.CreateInstance(type, this) as IView;
            res.ViewModel = viewModel;
            NextView = res;
            return res;
        }

        //public void ResetVM(IView view, int id)
        //{
        //    if (typeof (IEntytyObjectVM).IsAssignableFrom(view.VmType))
        //    {
        //        var oldVM = (view.ViewModel as IEntytyObjectVM);
        //        if (!oldVM.SavedInContext)
        //            return;
        //        if (!oldVM.Validate())
        //            return;
        //        oldVM.SaveDB();

        //        var vm = Use<IPool>().GetDBVM(view.VmType, id) ?? Use<IPool>().CreateDBObject(view.VmType);
        //        view.ViewModel = vm;
        //    }
        //    else
        //    {
        //        view.ViewModel = Use<IPool>().GetOrCreateVM(view.VmType, id);
        //    }
        //}
        public T CreateView<T>(IViewModel viewModel)  where  T:IView
        {
            return (T)CreateView(typeof (T), viewModel) ;
        }

        public IView CreateView(Type type,int Id=-1) 
        {
            var res = Activator.CreateInstance(type, this) as IView;
            res.ViewModel = Use<IPool>().GetOrCreateVM(res.VmType,Id);
            NextView = res;
            return res;
        }
        public IView NextView { get; set; }
        public void ShowMessage(string res)
        {
            MessageBox.Show(res);
        }

        public void CloseView(IView view)
        {
            //if (!typeof (IEntytyObjectVM).IsAssignableFrom(view.VmType))
            //    return;
            //var oldVM = (view.ViewModel as IEntytyObjectVM);
            //if (!oldVM.SavedInContext)
            //    oldVM.Delete();
        }

        public IView CreateView(IViewModel viewModel)
        {
            if (!VmToViewMap.ContainsKey(viewModel.GetType()))
                return null;
            return CreateView(VmToViewMap[viewModel.GetType()], viewModel);
        }
    }


}