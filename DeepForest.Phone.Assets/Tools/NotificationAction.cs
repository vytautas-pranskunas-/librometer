using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace DeepForest.Phone.Assets.Tools
{
    /// <summary>
    /// Represents a <see cref="NotificationTool"/> user action.
    /// </summary>
    public class NotificationAction
    {
        #region Properties

        public object Content { get; private set; }
        public object ContentTemplateKey { get; private set; }

        internal ICommand Command
        {
            get;
            private set;
        }

        #endregion

        #region NotificationCommand

        private class NotificationCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;
            private readonly bool _closeBeforeExecute;

            public NotificationCommand(Action execute, Func<bool> canExecute, bool closeBeforeExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
                _closeBeforeExecute = closeBeforeExecute;
            }

            bool ICommand.CanExecute(object parameter)
            {
                return _canExecute();
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                add { }
                remove { }
            }

            void ICommand.Execute(object parameter)
            {
                if (_closeBeforeExecute)
                    NotificationTool.Close();

                _execute?.Invoke();

                if (_closeBeforeExecute)
                    return;

                NotificationTool.Close();
            }
        }
        
        #endregion

        #region Ctor

        public NotificationAction(object content, Action execute)
            : this(content, null, execute, () => true)
        {
        }

        public NotificationAction(object content, Action execute, bool closeBeforeExecute)
            : this(content, (object)null, execute, (Func<bool>)(() => true), closeBeforeExecute)
        {
        }

        public NotificationAction(object content, Action execute, Func<bool> canExecute)
            : this(content, null, execute, canExecute)
        {
        }

        public NotificationAction(object content, Action execute, Func<bool> canExecute, bool closeBeforeExecute = false)
            : this(content, (object)null, execute, canExecute, closeBeforeExecute)
        {
        }

        public NotificationAction(object content, object contentTemplateKey, Action execute)
            : this(content, contentTemplateKey, execute, () => true)
        {
        }

        public NotificationAction(object content, object contentTemplateKey, Action execute, Func<bool> canExecute, bool closeBeforeExecute = false)
        {
            Content = content;
            ContentTemplateKey = contentTemplateKey;
            Command = new NotificationCommand(execute, canExecute, closeBeforeExecute);
        }

        #endregion
    }
}
