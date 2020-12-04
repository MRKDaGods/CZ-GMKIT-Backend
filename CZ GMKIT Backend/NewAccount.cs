using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace MRK
{
    public partial class NewAccount : MetroForm
    {
        public enum AccountCreationResult
        {
            Cancel,
            Create
        }

        public delegate void AccountCreationCallback(AccountCreationResult result, string username, string nickname);

        AccountCreationCallback m_CreationCallback;
        Main m_Main;

        public NewAccount(Main main, AccountCreationCallback creationCallback, bool isEx = false, string user = "username")
        {
            InitializeComponent();
            m_Main = main;
            m_CreationCallback = creationCallback;

            cancelB.Click += (o, e) => SendCallback(AccountCreationResult.Cancel);
            createB.Click += (o, e) => SendCallback(AccountCreationResult.Create);

            if (isEx)
                AdequateForEx(user);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            m_Main.Enabled = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            m_Main.Enabled = true;
        }

        void SendCallback(AccountCreationResult result)
        {
            m_CreationCallback(result, userTB.Text, nickTB.Text);
            Close();
        }

        void AdequateForEx(string user) {
            Text = "Change nickname";
            createB.Text = "Set";

            userTB.Text = user;
            userTB.Enabled = false;
        }
    }
}
