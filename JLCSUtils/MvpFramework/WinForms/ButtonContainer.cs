using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Binding;
using MvpFramework.WinForms.Util;
using System.Diagnostics;
using MvpFramework.Menu;
using JohnLambe.Util;
using JohnLambe.Util.Reflection;
using MvpFramework.WinForms.Controls;
using JohnLambe.Util.Types;

namespace MvpFramework.WinForms
{
    //TODO: Pop-up menu for associated control, and commands that don't fit as buttons.

    /// <summary>
    /// A control that holds a set of buttons, provided as an <see cref="IOptionCollection"/>,
    /// or generated based on a Presenter.
    /// </summary>
    public partial class ButtonContainer : UserControl, IControlBinderExt, IOptionUpdate, IKeyboardKeyHandler
    {
        public ButtonContainer()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            if (_nextButtonCoords.IsEmpty && DesignMode)
                SetUpDesignMode();
        }

        public virtual void MvpRefresh()
        {
            Refresh();
        }

        protected virtual void SetUpDesignMode()
        {
            ClearButtons();

            // Dummy buttons to see the layout:

            SuspendLayout();
            /*
                        AddButton(new MenuItemModel()
                        {
                            Attribute = new MvpHandlerAttribute()
                            {
                                DisplayName = "Button 1"
                            }
                        }
                            );
                        AddButton(new HandlerResolver.Handler()
                        {
                            Attribute = new MvpHandlerAttribute()
                            {
                                DisplayName = "B2"
                            }
                        }
                            );
                        AddButton(new HandlerResolver.Handler()
                        {
                            Attribute = new MvpHandlerAttribute()
                            {
                                DisplayName = "Button 3"
                            }
                        }
                            );
            */
            ResumeLayout();
        }

        #region IControlBinder

        public virtual void BindModel(ModelBinderWrapper modelBinder, IPresenter presenter)
        {
            BindModel(modelBinder, new PresenterBinderWrapper(presenter));
        }

        public virtual void BindModel(ModelBinderWrapper modelBinder, PresenterBinderWrapper presenterBinder)
        {
            PopulateButtons(presenterBinder);
        }

        /// <summary>
        /// Control bound by the <see cref="IControlBinder"/>.
        /// </summary>
        [Browsable(false)]
        public virtual object BoundControl => this;

        #endregion

        #region Population

        protected virtual void Populate()
        {
            if (!string.IsNullOrEmpty(Filter))
            {

            }
            PopulateButtons(Buttons);
        }

        protected virtual void PopulateButtons(PresenterBinderWrapper presenterBinder)
        {
            Buttons = new OptionCollectionBuilder().Build(presenterBinder.Presenter, Filter);
            PopulateButtons(Buttons);
        }

        protected virtual void PopulateButtons(IOptionCollection buttons)
        {
            ClearButtons();

            if (buttons != null)
            {
                // Determine initial position:
                var insideRectangle = this.ClientInsideMarginsRectangle();
                switch (Orientation)
                {
                    //TODO: ButtonAlignment; Expand width if size setting is 0.
                    //| Could support MvpUiAttribute.Alignment.
                    case Orientation.Horizontal:
                        _nextButtonCoords.Y = insideRectangle.Top;
                        //CalcAlignedPosition(ButtonAlignment,insideRectangle.Height,EffectiveButtonHeight);
                        if (!ReverseDirection)
                            _nextButtonCoords.X = insideRectangle.Left;
                        else
                            _nextButtonCoords.X = insideRectangle.Right;
                        break;
                    case Orientation.Vertical:
                        _nextButtonCoords.X = insideRectangle.Left;
                        if (!ReverseDirection)
                            _nextButtonCoords.Y = insideRectangle.Top;
                        else
                            _nextButtonCoords.Y = insideRectangle.Bottom;
                        break;
                }

                // Create the buttons:
                SuspendLayout();
                try
                {
                    var buttonSorted = buttons.Children;
                    if (ReverseButtonOrder)
                        buttonSorted = buttonSorted.Reverse();
                    int index = 0x40000000;
                    foreach (var button in buttonSorted)
                    {
                        AddButton(button, index += ReverseDirection ? -1 : 1);
                    }
                }
                finally
                {
                    ResumeLayout();
                }
            }

            ModelChanged = false;
        }

        /// <summary>
        /// Remove all buttons from this control.
        /// </summary>
        protected virtual void ClearButtons()
        {
            while (Controls.Count > 0)
            {
                Controls[0].Dispose();
            }
            _nextButtonCoords = new Point();
            ModelChanged = false;

            // Remove handlers for changes to the model:
            if (Buttons != null)
            {
                foreach (var buttonModel in Buttons.Children)
                    buttonModel.Changed -= ButtonModel_Changed;
            }
        }

        /// <summary>
        /// Add a button to the collection.
        /// </summary>
        /// <param name="buttonModel"></param>
        /// <param name="index">Index in the tab order.</param>
        protected virtual void AddButton(MenuItemModel buttonModel, int index)
        {
            if (buttonModel.Visible)
            {
                Control button = CreateButtonControl(buttonModel);

                button.Click += (sender, args) => Button_Click(sender, new ButtonClickedEventArgs(buttonModel));
                button.TabIndex = index;

                buttonModel.Tag = button;

                PopulateButton(button, buttonModel);

                if (button.Parent == null)   // if an overridden PopulateButton assigns Parent, keep its value
                    button.Parent = this;
            }

            buttonModel.Changed += ButtonModel_Changed;   // attach this handler even if not visible, so that we will be notified if it becomes Visible
        }

        #region Updates

        /// <summary>
        /// Fired when the model of a button changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void ButtonModel_Changed(MenuItemModel sender, MenuItemModel.ChangedEventArgs args)
        {
            ModelChanged = true;
        }

        /// <summary>
        /// After calling this, updates to the model do not cause an update to the UI
        /// until a subsequent call to <see cref="ResumeUpdate"/>.
        /// Calls are nested, so <see cref="ResumeUpdate"/> has to be called the same number of times as this,
        /// to reenable updates.
        /// </summary>
        public virtual void SuspendUpdate()
        {
            UpdatesSuspendedCount++;
        }

        /// <summary>
        /// Resumes updates after a call to <see cref="SuspendUpdate"/>.
        /// </summary>
        public virtual void ResumeUpdate()
        {
            UpdatesSuspendedCount--;
            if (UpdatesSuspendedCount == 0)
            {
                RefreshIfUpdated();
            }
            if (UpdatesSuspendedCount < 0)
                UpdatesSuspendedCount = 0;
        }

        /// <summary>
        /// Refreshes the buttons from the model if they are flagged as updated (by <see cref="ModelChanged"/>).
        /// </summary>
        protected virtual void RefreshIfUpdated()
        {
            if (ModelChanged)
                Populate();
        }

        /// <summary>
        /// The count of nested calls to <see cref="SuspendUpdate"/> (0 if not suspended).
        /// </summary>
        protected int UpdatesSuspendedCount { get; private set; }

        /// <summary>
        /// True iff the model is flagged as changed.
        /// </summary>
        /// <seealso cref="SuspendUpdate"/>
        /// <seealso cref="RefreshIfUpdated"/>
        protected virtual bool ModelChanged { get; set; }

        #endregion

        /// <summary>
        /// Handles a click on one of the buttons.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="args">Must be a <see cref="ButtonClickedEventArgs"/>.</param>
        protected virtual void Button_Click(object sender, EventArgs args)
        {
            var buttonClickedEventArgs = (ButtonClickedEventArgs)args;
            ButtonClicked?.Invoke(this, buttonClickedEventArgs);
            if (!buttonClickedEventArgs.Intercept)
                buttonClickedEventArgs.Button.Invoke();
        }

        /// <summary>
        /// Creates a new button control.
        /// This can be overridden to return custom button classes,
        /// for example, to integrate with third party UI components.
        /// </summary>
        /// <param name="handlerInfo"></param>
        /// <returns></returns>
        protected virtual Control CreateButtonControl(MenuItemModel handlerInfo)
        {
            return new MvpButton();
        }

        /// <summary>
        /// Set properties of a new button.
        /// Called after creating the button and before placing it in the window (assigning its Parent).
        /// <para>Iff this assigns Parent (leaves it non-null on exit), it will not be assigned after this.
        /// (This can be used in subclasses to place it on a new control, such as a custom panel).</para>
        /// </summary>
        /// <param name="button"></param>
        /// <param name="buttonModel"></param>
        protected virtual void PopulateButton(Control button, MenuItemModel buttonModel)
        {
            bool hasIcon = !string.IsNullOrEmpty(buttonModel.IconId);

            if (button is Button)
            {
                (button as Button).UseVisualStyleBackColor = true;   // so that it doesn't use the color of the background.
                (button as Button).AutoSize = ButtonAutoSize;    // resizes if text is too long for the initial width. Doesn't reduce size if text is shorter.
                (button as Button).AutoSizeMode = ButtonAutoSizeMode;    // resizes if text is too long for the initial width. Doesn't reduce size if text is shorter.
            }

            int buttonWidth = button.Width;
            /*
            if (ButtonAutoSize)
            {
                int textWidth = (int)button.CreateGraphics().MeasureString(button.Text, button.Font).Width;
                buttonWidth = textWidth + (hasIcon ? 32 : 0) + button.Margin.Horizontal + 2;
            }
            */
            // We could adjust the height of buttons when vertically stacked and the text is more than one line (due to wrapping or line breaks).

            // Assign minimum width, before assigning text:
            button.Width = ButtonMinimumWidth != 0 ? ButtonMinimumWidth
                : Orientation == Orientation.Vertical ? this.ClientInsideMarginsRectangle().Width
                : buttonWidth;

            button.Text = buttonModel.DisplayName;    // may cause resizing

            button.Height = EffectiveButtonHeight(button.Height);

            button.Location = _nextButtonCoords;
            button.Anchor = ButtonAnchor;

            button.CausesValidation = buttonModel.CausesValidation;

            button.Enabled = buttonModel.Enabled;
            button.Visible = buttonModel.Visible;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    if (ReverseDirection)              // right to left
                        button.Left -= button.Width;   // align to the right from the starting position (_nextButtonCoords.X is the right edge)
                    _nextButtonCoords.X += (button.Width + ButtonSpacing) * DirectionMultiplier;
                    break;
                case Orientation.Vertical:
                    if (ReverseDirection)
                        button.Top -= button.Height;
                    _nextButtonCoords.Y += (button.Height + ButtonSpacing) * DirectionMultiplier;
                    break;
            }

            SetIcon(button, buttonModel.IconId);
        }

        protected virtual void SetIcon(Control button, string iconId)
        {
            IconUtil.SetIcon(button, iconId);
        }

        /// <summary>
        /// </summary>
        /// <param name="height">the default height of a button.</param>
        /// <returns>the height that the button should be taking account of the configuration.</returns>
        //| Could be public - might be useful for a form using this to adjust its layout to align something with the buttons.
        protected virtual int EffectiveButtonHeight(int height)
            => ButtonHeight != 0 ?
                ButtonHeight     // explicit height given
                : Orientation == Orientation.Horizontal ? Height - Margin.Top - Margin.Bottom   // fill the space between the vertical margins
                : height;

        /// <summary>
        /// <see cref="IOptionUpdate.UpdateOption(OptionUpdateArgs)"/>
        /// </summary>
        /// <param name="args"></param>
        public virtual void UpdateOption(OptionUpdateArgs args)
        {
            if (Buttons != null)
            {
                SuspendUpdate();
                foreach (var buttonModel in Buttons.Children)
                {
                    if (buttonModel.Matches(args.Id, args.Filter))
                        args.OnUpdate(new OptionUpdateContext(buttonModel));
                }
                ResumeUpdate();
            }
        }

        /// <summary>
        /// -1 for left to right or bottom to top, otherwise 1.
        /// </summary>
        protected virtual int DirectionMultiplier
            => ReverseDirection ? -1 : 1;

        /// <summary>
        /// The <see cref="Control.Anchor"/> value for each button.
        /// </summary>
        protected virtual AnchorStyles ButtonAnchor
        {
            get
            {
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        return AnchorStyles.Top
                            | (!ReverseDirection ? AnchorStyles.Left : AnchorStyles.Right);
                    case Orientation.Vertical:
                        return AnchorStyles.Left
                            | (!ReverseDirection ? AnchorStyles.Top : AnchorStyles.Bottom);
                    default:
                        throw new Exception("INTERNAL ERROR: Invalid Orientation");
                }
            }
        }

        /// <summary>
        /// The coordinates of the next button to be placed (during binding).
        /// </summary>
        protected Point _nextButtonCoords;

        #endregion

        #region Properties for Form Designer

        /// <summary>
        /// Filters handlers of the presenter.
        /// <para>null for all handlers.</para>
        /// </summary>
        /// <seealso cref="OnGetFilter"/>.
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Description("Filters handlers of the presenter. See OnGetFilter.")]
        //[DefaultValue(null)]   // with no default, Form Designer can't set to null.
        public virtual string Filter
        {
            get
            {
                string filter = OnGetFilter?.Invoke(this, GetNameEventArgs.Empty);
                if (filter == FilterAll)
                    return null;
                else
                    return filter
                        ?? Name?.RemovePrefix("ui")
                        ?? "";
            }
        }

        /// <summary>
        /// Value returned from <see cref="OnGetFilter"/> to match all handlers of the presenter.
        /// </summary>
        public const string FilterAll = "*";
        //TODO: Special value for None ?

        /// <summary>
        /// Returns the <see cref="Filter"/> value to filter handlers of the Presenter.
        /// If this does not have a handler, the Filter is name of this control, with a prefix of "ui" removed (if present).
        /// </summary>
        /// <seealso cref="FilterAll"/>
        /// <remarks>
        /// Using an event handler allows returning a constant that can be referenced on the handlers (in the <see cref="MvpHandlerAttribute"/>),
        /// thus avoiding using a string literal that would have to match.
        /// </remarks>
        [Category(MvpUiComponentConsts.DesignerCategory)]
        [Description("Returns the Filter value to filter handlers of the Presenter.\n"
            + "If this does not have a handler, or it returns null, the Filter is name of this control, with a prefix of \"ui\" removed (if present)."
            + "Using an event handler allows assigning it to a constant that can be referenced on the handlers.\n"
            + "Return " + nameof(ButtonContainer) + "." + nameof(FilterAll) + " (\"" + FilterAll + "\") to match all handlers.")]
        public virtual event GetNameDelegate OnGetFilter;

        /// <summary>
        /// Default and minimum width of each button, in pixels.
        /// </summary>
        [Category("Layout")]
        [DefaultValue(DefaultMinimumButtonWidth)]
        [Description("Default width of each button, in pixels. "
                    + "0 for default, which makes them fill the width of this control if Orientation is Vertical.")]
        public virtual int ButtonMinimumWidth { get; set; } = DefaultMinimumButtonWidth;
        protected const int DefaultMinimumButtonWidth = 0;

        /// <summary>
        /// Space between buttons, in pixels.
        /// </summary>
        //| Could use Padding instead, but it might not be intuitive since most of its properties would not be relevant.
        [Category("Layout")]
        [DefaultValue(DefaultButtonSpacing)]
        [Description("Space between buttons, in pixels.")]
        public virtual int ButtonSpacing { get; set; } = DefaultButtonSpacing;
        public const int DefaultButtonSpacing = 8;

        /// <summary>
        /// Whether the buttons are positioned in a horizontal line or stacked vertically.
        /// </summary>
        [Category("Layout")]
        [Description("Whether the buttons are positioned in a horizontal line or stacked vertically, and which side they start at.")]
        [DefaultValue(TabAlignment.Top)]
        public virtual TabAlignment ButtonsLayout { get; set; } = TabAlignment.Top;
        //TODO: Doesn't work yet.

        [Category("Layout")]
        [Description("Whether the buttons are positioned in a horizontal line or stacked vertically.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual Orientation Orientation
            => (ButtonsLayout == TabAlignment.Left || ButtonsLayout == TabAlignment.Right) ? Orientation.Horizontal : Orientation.Vertical;

        /// <summary>
        /// Place buttons from right to left or bottom to top (depending on <see cref="Orientation"/>).
        /// </summary>
        [Category("Layout")]
        [Description("Place buttons from right to left or bottom to top (depending on Orientation).")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual bool ReverseDirection
            => ButtonsLayout.HasFlag(TabAlignment.Bottom | TabAlignment.Right);

        [Category("Layout")]
        [Description("Sort the buttons in the opposite order.")]
        [DefaultValue(false)]
        public virtual bool ReverseButtonOrder { get; set; } = false;

        [Category("Layout")]
        [Description("Alignment of buttons horizontally if the Orientation is Vertical, otherwise the equivalent vertical alignment is applied.")]
        [DefaultValue(HorizontalAlignment.Center)]
        public virtual HorizontalAlignment ButtonAlignment { get; set; } = HorizontalAlignment.Center;
        //TODO: Doesn't work yet.

        [Category("Layout")]
        [Description("Height of each button, in pixels. 0 for default, which makes them fill the height of this control if Orientation is Horizontal.")]
        [DefaultValue(0)]
        public virtual int ButtonHeight { get; set; } = 0;

        [Category("Layout")]
        [Description("True to automatically size the buttons. Currently supported for horizontal layouts only.")]
        [DefaultValue(true)]
        public virtual bool ButtonAutoSize { get; set; } = true;

        [Category("Layout")]
        [Description("How buttons automatically size (if ButtonAutoSize is true).")]
        [DefaultValue(AutoSizeMode.GrowOnly)]
        public virtual AutoSizeMode ButtonAutoSizeMode { get; set; } = AutoSizeMode.GrowOnly;

        [Category("Action")]
        [Description("Fired when any of the buttons is clicked.")]
        public virtual event ButtonClickedDelegate ButtonClicked;

        [Nullable]
        [Description("Control that shares any popup menu with this panel, and keystrokes in this control may invoke commands in this panel.")]
        [DefaultValue(null)]
        public virtual Control AssociatedControl { get; set; }  //TODO: Implement this.
        //TODO: Rename?

        #endregion

        public virtual void SetupLinkedControl()
        {
            AssociatedControl.KeyPress += AssociatedControl_KeyPress;
            AssociatedControl.KeyDown += AssociatedControl_KeyDown;
        }

        protected virtual void AssociatedControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                InvokeDefault();
            //TODO: Process other keys; Context Keys
        }

        protected virtual void AssociatedControl_KeyDown(object sender, KeyEventArgs e)
        {
            //TODO: Process other keys; Context Keys
        }

        /// <summary>
        /// Invoke the default action.
        /// </summary>
        public virtual void InvokeDefault()
        {
            Buttons?.Default?.Invoke();
        }

        public virtual void NotifyKeyDown(KeyboardKeyEventArgs args)
        {
            args.Cancel = Buttons.ProcessKey(args.Key);
        }

        /// <summary>
        /// The collection of buttons.
        /// </summary>
        [Browsable(false)]
        public virtual IOptionCollection Buttons
        {
            get { return _buttons; }
            set
            {
                _buttons = value;
                PopulateButtons(Buttons);
            }
        }
        protected IOptionCollection _buttons;

        /// <summary>
        /// Arguments for <see cref="ButtonClickedDelegate"/>.
        /// </summary>
        public class ButtonClickedEventArgs : EventArgs
        {
            /// <summary>
            /// </summary>
            /// <param name="button">Value of <see cref="Button"/>.</param>
            public ButtonClickedEventArgs(MenuItemModel button)
            {
                this.Button = button;
            }

            /// <summary>
            /// The model of the button that was clicked.
            /// </summary>
            public virtual MenuItemModel Button { get; }

            /// <summary>
            /// Set to true to prevent the event from passed to later handlers.
            /// </summary>
            public virtual bool Intercept { get; set; }
        }

        /// <summary>
        /// Event fired on clicking a button in a <see cref="ButtonContainer"/>.
        /// </summary>
        /// <param name="sender">The button container.</param>
        /// <param name="args"></param>
        public delegate void ButtonClickedDelegate(object sender, ButtonClickedEventArgs args);
    }


    //TODO: Replace with System.Drawing.ContentAlignment ?
    /// <summary>
    /// A layout and orientation of a row of buttons.
    /// </summary>
    /// <seealso cref="ButtonContainer"/>
    public enum ButtonContainerLayout
    {
        /// <summary>
        /// Horizontal row of buttons from left to right.
        /// </summary>
        LeftToRight = 1,
        /// <summary>
        /// Horizontal row of buttons from right to left.
        /// </summary>
        RightToLeft,
        /// <summary>
        /// Vertical row of buttons from top to bottom.
        /// </summary>
        TopToBottom,
        /// <summary>
        /// Vertical row of buttons from bottom to top.
        /// </summary>
        BottomToTop,
        /// <summary>
        /// Horizontal row of buttons, centred.
        /// </summary>
        HorizontalCentred,
        /// <summary>
        /// Vertical row of buttons, centred.
        /// </summary>
        VerticalCentred
    }

}
