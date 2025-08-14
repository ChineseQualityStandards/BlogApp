using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class ArticleEditorViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        #endregion

        #region 属性

        public bool KeepAlive => true;

        private string? _MdText;
        public string? MdText
        {
            get => _MdText;
            set => SetProperty(ref _MdText, value);
        }

        #endregion

        #region 命令

        #endregion

        #region 函数

        public ArticleEditorViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _regionManager = regionManager;
        }

        private void DelegateMethod(string command)
        {
            //MessageBox.Show(command);
        }

        #endregion
    }
}
