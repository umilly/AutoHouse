using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;
using VMBase;

namespace View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ClientParametersView
    {
        public  ClientParametersView()
        {
            InitializeComponent();
        }

        public  ClientParametersView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        private readonly List<ClientParameterContainerView> _children=new List<ClientParameterContainerView>();
        public List<ClientParameterContainerView> ParameterViewList
        {
            get
            {
                PrepareChildren();
                return _children;
            }
        }

        private void PrepareChildren()
        {
            if (ViewModel == null)
            {
                _children.Clear();
                return;
            }
            _children.FullFill(ViewModel.Parameters.Count(),() => _viewService.CreateView<ClientParameterContainerView>(null));
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].ViewModel= ViewModel.Parameters[i];
            }
        }

        public override void OnVMSet()
        {
            base.OnVMSet();
            ViewModel.AskParams(() =>
            {
                OnPropertyChanged(()=>ParameterViewList);
            });
            OnPropertyChanged(() => ParameterViewList);
        }
    }

    public static class ListExt
    {
        public static void FullFill<T>(this List<T> src,int count,Func<T> creationMethod)
        {
            var curCount = src.Count();
            if(curCount==count||count<0)
                return;
            if (curCount > count)
            {
                src.RemoveRange(curCount-1,curCount-count);
            }
            if (curCount < count)
            {
                for (int i = 0; i < count-curCount; i++)
                {
                    src.Add(creationMethod());
                }
            }
        }
    }
}

