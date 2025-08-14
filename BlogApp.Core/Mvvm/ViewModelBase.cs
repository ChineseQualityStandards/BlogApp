using BlogApp.Core.Models;

namespace BlogApp.Core.Mvvm
{
    public abstract class ViewModelBase : Messages, IDestructible
    {
        #region 方法

        /// <summary>
        /// ViewModelBase构造函数
        /// </summary>
        protected ViewModelBase()
        {

        }

        /// <summary>
        /// 销毁函数
        /// </summary>
        public virtual void Destroy() { }

        /// <summary>
        /// 加载函数
        /// </summary>
        public virtual void Load() { }

        /// <summary>
        /// 清理函数
        /// </summary>
        public virtual void Clear() { }

        /// <summary>
        /// 设置错误消息
        /// </summary>
        /// <param name="theMessage">错误消息</param>
        public void SetErrorMessage(string theMessage) => ErrorMessage = theMessage;
        /// <summary>
        /// 设置普通消息
        /// </summary>
        /// <param name="theMessage">普通消息</param>
        public void SetMessage(string theMessage) => Message = theMessage;
        /// <summary>
        /// 设置警告消息
        /// </summary>
        /// <param name="theMessage">警告消息</param>
        public void SetWarningMessage(string theMessage) => WarningMessage = theMessage;

        #endregion
    }
}
