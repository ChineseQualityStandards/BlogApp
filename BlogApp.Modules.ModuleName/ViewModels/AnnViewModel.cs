using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.Mvvm;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class AnnViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        #endregion

        #region 属性

        public bool KeepAlive => true;

        #endregion

        #region 命令

        #endregion

        #region 函数

        public AnnViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _regionManager = regionManager;
        }


        #endregion
    }
}
