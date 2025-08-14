using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class BookContentViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;

        #endregion

        #region 属性

        public bool KeepAlive => true;

        private ObservableCollection<Chapter>? list;
        /// <summary>
        /// 章节列表
        /// </summary>
        public ObservableCollection<Chapter>? List
        {
            get { return list; }
            set { SetProperty(ref list, value); }
        }

        private string? _MdText;
        public string? MdText
        {
            get => _MdText;
            set => SetProperty(ref _MdText, value);
        }

        #endregion

            #region 命令

        private Chapter? selectedChapter;
        public Chapter? SelectedChapter
        {
            get { return selectedChapter; }
            set
            {
                SetProperty(ref selectedChapter, value);
                if (value != null)
                {
                    DelegateMethod(value.IdToString);
                }
            }
        }

        #endregion

        #region 函数

        public BookContentViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _regionManager = regionManager;



            List = new ObservableCollection<Chapter>()
            {
                new Chapter (){ Id=0, IdToString="0", Title = "XAML概览" },
                new Chapter (){ Id=0, IdToString="1", Title = "XAML是什么" },
                new Chapter (){ Id=0, IdToString="2", Title = "XAML的有点" },
            };

        }

        private void DelegateMethod(string command)
        {
            //MessageBox.Show(command);



        }

        #endregion
    }
}
