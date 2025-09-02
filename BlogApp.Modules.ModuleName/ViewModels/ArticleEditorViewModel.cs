using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml;
using BlogApp.Core.Constants;
using BlogApp.Core.Models;
using BlogApp.Core.Mvvm;
using BlogApp.Services.Interfaces;
using DryIoc;

namespace BlogApp.Modules.ModuleName.ViewModels
{
    public class ArticleEditorViewModel : RegionViewModelBase, IRegionMemberLifetime
    {
        #region 字段

        private readonly IRegionManager _regionManager;
        private readonly IArticleService _articleService;

        #endregion

        #region 属性

        public bool KeepAlive => false;

        private string? _MdText;
        public string? MdText
        {
            get => _MdText;
            set => SetProperty(ref _MdText, value);
        }

        private Article? _Article;

        public Article? Article
        {
            get { return _Article; }
            set { SetProperty(ref _Article, value); }
        }


        #endregion

        #region 命令

        public DelegateCommand SendCommand { get; set; }

        #endregion

        #region 函数

        public ArticleEditorViewModel(IRegionManager regionManager, IArticleService articleService) : base(regionManager)
        {
            _regionManager = regionManager;
            _articleService = articleService;
            SendCommand = new DelegateCommand(SendMethod);
        }

        private async void SendMethod()
        {
            if (Article != null) 
            {
                if(Article.Id == 0)
                {
                    if (string.IsNullOrEmpty(Article.Title))
                    {
                        SetMessage("标题不能为空");
                        return;
                    }
                    if (string.IsNullOrEmpty(Article.Text))
                    {
                        SetMessage("内容不能为空");
                        return;
                    }
                    Article.CreatedDate = DateTime.Now;
                    Article.UpdatedDate = Article.CreatedDate;
                    var result = await _articleService.AddArticleAsync(Article);
                }
                else
                {
                    if (string.IsNullOrEmpty(Article.Title))
                    {
                        SetMessage("标题不能为空");
                        return;
                    }
                    if (string.IsNullOrEmpty(Article.Text))
                    {
                        SetMessage("内容不能为空");
                        return;
                    }
                    Article.UpdatedDate = DateTime.Now;
                    var result = _articleService.Update(Article);
                }
                // 创建导航参数，包含BookId
                var parameters = new NavigationParameters
                {
                    { "BookId", Article.BookId }
                };
                // 导航到BookContentView并传递参数
                _regionManager.RequestNavigate(RegionNames.ContentRegion, "BookContentView", parameters);
            }
        }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            // 从导航参数中获取BookId
            if (navigationContext.Parameters.ContainsKey("Article"))
            {
                Article = navigationContext.Parameters.GetValue<Article>("Article");
            }
            else
            {
                MessageBox.Show("新建文章失败，并没有找到传入实体");
                _regionManager.RequestNavigate(RegionNames.ContentRegion, "BookShelfView");
            }
        }
        #endregion
    }
}
