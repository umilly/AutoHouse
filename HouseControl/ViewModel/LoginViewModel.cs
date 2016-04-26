using System;
using System.Collections.Generic;
using Facade;
using Model;
using System.Linq;

namespace ViewModel
{
    public class LoginViewModel : ViewModelBase.ViewModelBase
    {
        public LoginViewModel(Models context) : base(context)
        {

        }
        private string _login;
        private string _pass;

        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public string Pass
        {
            get { return _pass; }
            set
            {
                _pass = value;
                OnPropertyChanged();
            }
        }

        public RoleType Role
        {
            get
            {
                if (_user != null && _user.role != null)
                    return (RoleType)_user.role;

                return RoleType.Undefined; ;
            }
        }

        private User _user;
        public bool TryToAuthorize()
        {
            _user = Context.Users.FirstOrDefault(f => f.login == Login && f.password == Pass);
            return _user != null;
        }
    }
}
