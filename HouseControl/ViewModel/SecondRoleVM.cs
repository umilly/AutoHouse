using Facade;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel
{
    public class SecondRoleVM : ViewModelBase.ViewModelBase
    {
        public SecondRoleVM(Models context) : base(context)
        {
        }
        public IList<IWrapper> Contents
        {
            get { return _contents.ToList(); }
        }

        private IEnumerable<IWrapper> _contents;

        public void LoadData()
        {
            _contents = Context.Contents.
                Select(s => new ContentViewEntity { Id = s.Id, Content = s.Content1 });
            OnPropertyChanged("Contents");
        }
    }

    public class ContentViewEntity : IWrapper
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }

}
