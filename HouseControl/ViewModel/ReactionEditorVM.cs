﻿using System.Collections.Generic;
using Facade;
using Model;
using ViewModelBase;

namespace ViewModel
{
    public class ReactionEditorVM : ViewModelBase.ViewModelBase
    {
        public ReactionEditorVM(IServiceContainer container) : base(container)
        {
        }

        public override int ID
        {
            get { return 1; }
            set { }
        }

        public IEnumerable<ITreeNode> Reactions
        {
            get { yield return Use<IPool>().GetOrCreateDBVM<SystemViewModel>(EmptyModel.Instance); }
        }
    }
}