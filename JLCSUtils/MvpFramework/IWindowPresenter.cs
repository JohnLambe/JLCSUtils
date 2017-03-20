using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// A presenter that has its own window.
    /// </summary>
    public interface IWindowPresenter<TModelResult> : IPresenter
    {
        /// <summary>
        /// Show window modally.
        /// </summary>
        /// <returns>Depends on the presenter.</returns>
        TModelResult ShowModal();

        /// <summary>
        /// Close the window.
        /// </summary>
        void Close();
    }


    /// <summary>
    /// Non-generic equivalent of <see cref="IWindowPresenter{TModelResult}"/>.
    /// </summary>
    public interface IWindowPresenter : IWindowPresenter<object>
    {
    }


    /// <summary>
    /// Event fired when a closes, or before it closes, allowing handlers to prevent it from closing.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ViewVisibilityChangedDelegate(object sender, ViewVisibilityChangedEventArgs args);

    /// <summary>
    /// Arguments to the <see cref="ViewVisibilityChangedDelegate"/> event.
    /// </summary>
    public class ViewVisibilityChangedEventArgs
    {
        public ViewVisibilityChangedEventArgs(VisibilityChange action)
        {
            this.Action = action;
        }

        public virtual VisibilityChange Action { get; protected set; }

        /// <summary>
        /// Iff true, the form is closed.
        /// </summary>
        public virtual bool Closed => Action == VisibilityChange.Closed;

        /// <summary>
        /// true iff the View is modal.
        /// </summary>
        public virtual bool IsModal { get; set; }

        /// <summary>
        /// Set to true to prevent the view from closing.
        /// </summary>
        public virtual bool Intercept
        {
            get { return _intercept; }
            set
            {
                if (Action != VisibilityChange.Closing)
                    throw new InvalidOperationException("This ViewVisibilityChangedEvent is not interceptable");
                _intercept = value;
            }
        }

        protected bool _intercept = false;
    }

    public enum VisibilityChange
    {
        Opened = 10,
        Closing = 20,
        Closed = 25
    }
}
