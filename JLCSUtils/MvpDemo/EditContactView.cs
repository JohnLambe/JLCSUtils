using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JohnLambe.Util;
using MvpFramework;
using MvpFramework.WinForms;
using MvpFramework.Binding;
using MvpDemo.Model;
using MvpFramework.Dialog;

namespace MvpDemo
{
    [View]
    public partial class EditContactView : WindowViewBase, IEditContactView
    {
        public EditContactView(IMessageDialogService dialogService = null) : base(dialogService)
        {
            InitializeComponent();
            //Text = "Edit Contact Window";
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            //            MessageBox.Show("Validating " + (sender as Control).Name);
            //ViewBinder.InvalidateView();
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
//            MessageBox.Show("Validated " + e.ToString() + " " + (sender as Control).Name);

        }

        private void EditContactView_ViewVisibilityChanged(object sender, ViewVisibilityChangedEventArgs args)
        {
//            MessageBox.Show("ViewVisibilityChanged");
        }

        private void textBox2_ModifiedChanged(object sender, EventArgs e)
        {
//            MessageBox.Show("ModifiedChanged " + (sender as Control).Name + " " + (sender as TextBoxBase).Modified);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            uiAddress.Modified = false;
            MessageBox.Show(ItemType + "\n" + Model1.ToString());
            TestEvent?.Invoke(this, EventArgs.Empty);

            Test2Event?.Invoke(this, e);

            Model1.PhoneNumber = "A";  // invalid
        }

        [MvpBind("EntityDescription")]
        public virtual string ItemType { get; set; }

        [MvpBind(MvpBindAttribute.Model)]
        public Contact Model1 { get; set; }

        [MvpEvent("Test")]
        public event EventHandler TestEvent;

        [MvpEvent]
        public event EventHandler Test2Event;

        [MvpEvent("Test_NoHandler")]
#pragma warning disable CS0067   // event never used
        public event EventHandler TestEvent2;
#pragma warning restore CS0067

        private string buttonContainer2_OnGetFilter(object sender, EventArgs args)
        {
            return "";// ButtonContainer.FilterAll;
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            ViewBinder.ValidateModel();
        }

        private void uiValidateControls_Click(object sender, EventArgs e)
        {
//            ViewBinder.ValidateControls();
        }

        [MvpHandler(SingleFilter = BindingConsts.Filter_Keys, HotKey = KeyboardKey.F5)]
        public virtual void HandleF5()
        {
            MessageBox.Show("F5");
        }

        private void button2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 'A')
                MessageBox.Show("" + e.KeyChar);
        }

        private void EditContactView_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void EditContactView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Home || (e.KeyCode == (Keys.Control | Keys.U)))
            {
                MessageBox.Show("" + e.KeyCode + "; " + e.IsInputKey);
                e.IsInputKey = true;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F12))
                MessageBox.Show("ProcessDialogKey: " + keyData.ToString());

            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F12))
                MessageBox.Show("ProcessCmdKey: " + keyData.ToString());

            return base.ProcessCmdKey(ref msg, keyData);
        }

        //
        // Summary:
        //     Processes Windows messages.
        //
        // Parameters:
        //   m:
        //     The Windows System.Windows.Forms.Message to process.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void WndProc(ref Message m)
        {
            if(m.Msg == WM_KEYDOWN)
            {
                Console.Out.WriteLine(m.LParam);
            }
            base.WndProc(ref m);
        }

        const int WM_KEYDOWN = 0x100;

    }
}
