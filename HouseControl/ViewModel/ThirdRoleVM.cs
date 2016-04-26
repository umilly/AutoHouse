using Model;

namespace ViewModel
{
    public class ThirdRoleVM : ViewModelBase.ViewModelBase
    {
        private string _firstField;
        private string _secondField;

        public ThirdRoleVM(Models context) : base(context)
        {

        }

        public string FirstField
        {
            get { return _firstField; }
            set
            {
                _firstField = value;
                OnPropertyChanged();
            }
        }

        public string SecondField
        {
            get { return _secondField; }
            set
            {
                _secondField = value;
                OnPropertyChanged();
            }
        }

        public void IncreaseFirstField()
        {
            FirstField += "A";
        }

        public void IncreaseSecondField()
        {
            SecondField += "B";
        }

    }
}
