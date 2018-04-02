using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Misc;
using JohnLambe.Util.Validation;

namespace MvpFramework
{
    /// <summary>
    /// Options relating to windows (independent of the UI framework) - usually top level windows or MDI child windows, but depending on the UI framework and components,
    /// some of them may be supported for other UI elements (such as tabs or panes).
    /// <para>
    /// Unless otherwise indicated, null values in this class cause a default to be used.
    /// UI frameworks and components may ignore values that they do not support.
    /// </para>
    /// </summary>
    public class WindowOptions
    {
        public virtual bool? CloseButton
        {
            get { return _closeButton; }
            set
            {
                _closeButton = value;
                FireChanged();
            }
        }
        private bool? _closeButton;

        public virtual bool? MinimizeButton
        {
            get { return _minimizeButton; }
            set
            {
                _minimizeButton = value;
                FireChanged();
            }
        }
        private bool? _minimizeButton;

        public virtual bool? MaximizeButton
        {
            get { return _maximizeButton; }
            set
            {
                _maximizeButton = value;
                FireChanged();
            }
        }
        private bool? _maximizeButton;

        public virtual bool? HelpButton
        {
            get { return _helpButton; }
            set
            {
                _helpButton = value;
                FireChanged();
            }
        }
        private bool? _helpButton;

        public virtual bool? ShowInTaskBar
        {
            get { return _showInTaskBar; }
            set
            {
                _showInTaskBar = value;
                FireChanged();
            }
        }
        private bool? _showInTaskBar;

        /// <summary>
        /// Iff true, this window is always on top of windows that don't have this option turned on.
        /// (The WinForms Form.TopMost option.)
        /// </summary>
        public virtual bool? StayOnTop
        {
            get { return _stayOnTop; }
            set
            {
                _stayOnTop = value;
                FireChanged();
            }
        }
        private bool? _stayOnTop;

        [IconId]
        public string IconId
        {
            get { return _iconId; }
            set
            {
                _iconId = value;
                FireChanged();
            }
        }
        private string _iconId;

        /// <summary>
        /// The opacity of the window.
        /// 1 for opaque. 0 for fully transparent (or the most transparent value that is supported).
        /// This may be ignored if the UI framework does not support it.
        /// </summary>
        [PercentageValidation]
        public decimal? Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                FireChanged();
            }
        }
        private decimal? _opacity;

        //TODO:
        //  BorderStyle
        //  StartPosition

        /// <summary>
        /// Fired when any public property of this instance changes.
        /// </summary>
        public event EventHandler Changed;

        protected void FireChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
