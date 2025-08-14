using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.Mvvm;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class ViewAViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        private readonly IRegionManager _regionManager;

        public bool KeepAlive => true;

        public ViewAViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _regionManager = regionManager;
            SetMessage("這是ViewA!");
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }
    }
}
