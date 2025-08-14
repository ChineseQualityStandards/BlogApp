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
        public virtual void Destroy()
        {

        }

        /// <summary>
        /// 设置错误消息
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        public void SetErrorMessage(string errorMessage) => ErrorMessage = errorMessage;
        /// <summary>
        /// 设置普通消息
        /// </summary>
        /// <param name="message">普通消息</param>
        public void SetMessage(string message) => Message = message;
        /// <summary>
        /// 设置警告消息
        /// </summary>
        /// <param name="warningMessage">警告消息</param>
        public void SetWarningMessage(string warningMessage) => WarningMessage = warningMessage;

        #endregion
    }
}
