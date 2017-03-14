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

namespace MvpFramework.WinForms
{
    public partial class ButtonContainer : UserControl, IControlBinderExt
    {
        public ButtonContainer()
        {
            InitializeComponent();            
        }

        public override void Refresh()
        {
            if (_nextButtonCoords.IsEmpty && DesignMode)
                SetupDesignMode();
        }

        protected virtual void SetupDesignMode()
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
            if(!string.IsNullOrEmpty(Filter))
            {

            }
            PopulateButtons(Buttons);
        }

        protected virtual void PopulateButtons(PresenterBinderWrapper presenterBinder)
        {
            PopulateButtons(new OptionCollectionBuilder().Build(presenterBinder.Presenter, Filter));
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
                    foreach (var button in buttonSorted)
                    {
                        AddButton(button);
                    }
                }
                finally
                {
                    ResumeLayout();
                }
            }
        }

        /// <summary>
        /// Remove all buttons from this control.
        /// </summary>
        protected virtual void ClearButtons()
        {
            while(Controls.Count > 0)
            {
                Controls[0].Dispose();
            }
            _nextButtonCoords = new Point();
        }

        /// <summary>
        /// Add a button to the collection.
        /// </summary>
        /// <param name="buttonModel"></param>
        protected virtual void AddButton(MenuItemModel buttonModel)
        {
            Control button = CreateButtonControl(buttonModel);

            button.Click += (sender, args) => Button_Click(sender, new ButtonClickedEventArgs(buttonModel));

            PopulateButton(button, buttonModel);

            if (button.Parent == null)   // if an overridden PopulateButton assigns Parent, keep its value
                button.Parent = this;
        }

        /// <summary>
        /// Handles a click on one of the buttons.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="args">Must be a <see cref="ButtonClickedEventArgs"/>.</param>
        protected virtual void Button_Click(object sender, EventArgs args)
        {
            var buttonClickedEventArgs = (ButtonClickedEventArgs)args;
            ButtonClicked?.Invoke(this, buttonClickedEventArgs);
            if(!buttonClickedEventArgs.Intercept)
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
            return new Button();
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
            if (button is Button)
            {
                (button as Button).UseVisualStyleBackColor = true;   // so that it doesn't use the color of the background.
                (button as Button).AutoSize = true;    // resizes if text is too long for the initial width. Doesn't reduce size if text is shorter.
            }

            // Assign minimum width, before assigning text:
            button.Width = MinimumButtonWidth != 0 ? MinimumButtonWidth
                : Orientation == Orientation.Vertical ? this.ClientInsideMarginsRectangle().Width
                : button.Width;

            button.Text = buttonModel.DisplayName;    // may cause resizing

            button.Height = EffectiveButtonHeight(button.Height);

            button.Location = _nextButtonCoords;
            button.Anchor = ButtonAnchor;

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

            //TODO: Icon
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
        /// </summary>
        [Category(MvpFrameworkConstants.MvpCategory)]
        [Description("Filters handles of the presenter.")]
        //[DefaultValue(null)]   // with no default, Form Designer can't set to null.
        public virtual string Filter { get; set; }

        /// <summary>
        /// Default width of each button, in pixels.
        /// </summary>
        [Category("Layout")]
        [DefaultValue(DefaultMinimumButtonWidth)]
        [Description("Default width of each button, in pixels. "
                    + "0 for default, which makes them fill the width of this control if Orientation is Vertical.")]
        public virtual int MinimumButtonWidth { get; set; } = DefaultMinimumButtonWidth;
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

        [Description("Whether the buttons are positioned in a horizontal line or stacked vertically.")]
        protected virtual Orientation Orientation
            => (ButtonsLayout == TabAlignment.Left || ButtonsLayout == TabAlignment.Right) ? Orientation.Horizontal : Orientation.Vertical;

        /// <summary>
        /// Place buttons from right to left or bottom to top (depending on <see cref="Orientation"/>).
        /// </summary>
        [Category("Layout")]
        [Description("Place buttons from right to left or bottom to top (depending on Orientation).")]
        [DefaultValue(false)]
        protected virtual bool ReverseDirection  //{ get; set; } = false;
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

        [Category("Action")]
        [Description("Fired when any of the buttons is clicked.")]
        public virtual event ButtonClickedDelegate ButtonClicked;

        #endregion

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
            public ButtonClickedEventArgs(MenuItemModel button)
            {
                this.Button = button;
            }

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

}
