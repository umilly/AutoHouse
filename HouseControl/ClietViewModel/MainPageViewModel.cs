using System.ComponentModel;


namespace ClietViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        string _someText;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            this.SomText = "хуйня муйня";
        }

        public string SomText
        {
            set
            {
                if (_someText != value)
                {
                    _someText = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("SomText"));
                    }
                }
            }
            get
            {
                return _someText;
            }
        }
    }
}
