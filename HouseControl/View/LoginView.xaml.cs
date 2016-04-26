using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Facade;
using System.Windows;
using ViewModel;
using VMBase;

namespace View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : CustomViewBase<LoginViewModel>
    {
        private static Dictionary<RoleType, Type> ViewByRole;

        static LoginView()
        {
             var assably=Assembly.GetAssembly(typeof(LoginView));
            var roleViews = assably.GetTypes().Where(a => Attribute.IsDefined(a, typeof (RoleAttribute)));
            ViewByRole = roleViews.ToDictionary(RoleAttribute.GetRoleForView, type => type);
        }

        public LoginView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            var list = File.ReadAllLines("D:\\input.txt");
            foreach (var file in list)
            {
                var newfile = file.Replace("F:\\work\\dragonlands_2\\", "F:\\work\\dragonlands\\");
                if (File.Exists(newfile))
                {
                    File.Delete(newfile);
                    File.WriteAllBytes(newfile,File.ReadAllBytes(file));
                }
            }
            return;
            if (!ViewModel.TryToAuthorize())
            {
                MessageBox.Show("Authorization failed");
                return;
            }
            if(!ViewByRole.ContainsKey(ViewModel.Role))
                throw new ArgumentOutOfRangeException(string.Format( "dont defined view for role {0}",ViewModel.Role));
            _viewService.NextView = _viewService.CreateView(ViewByRole[ViewModel.Role]);
            ShowNextViewCommand.Instance.Execute(this, this);
        }
    }
}
