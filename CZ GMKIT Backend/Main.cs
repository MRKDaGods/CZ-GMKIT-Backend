using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.IO;
using MetroFramework.Controls;

namespace MRK {
    public partial class Main : MetroForm {
        class SelectedItem {
            public string Item;
            public int Section;
        }

        Project m_Project;
        List<Account> m_Accounts;
        Color m_IdleColor;
        Color m_DownColor;
        int m_SelectedIndex;
        List<MetroPanel> m_ClearBuffer;
        List<string>[] m_ContextualItems;
        Panel m_SelectedCtxPanel;
        SelectedItem m_SelectedItem;
        int m_ActiveSection;

        Account CurrentAccount => m_SelectedIndex >= m_Accounts.Count || m_SelectedIndex < 0 ? null : m_Accounts[m_SelectedIndex];

        public Main() {
            InitializeComponent();

            selectB.Click += (o, e) => {
                SelectProject(@"E:\Backup\Unity\CZ3 Proxy");
                return;
                FolderBrowserDialog dialog = new FolderBrowserDialog();

                if (dialog.ShowDialog() == DialogResult.OK) {
                    SelectProject(dialog.SelectedPath);
                }
            };

            patchB.Click += (o, e) => {
                m_Project = Project.PatchProject(pathL.Text);

                m_Accounts.ForEach(x => {
                    x.IsDirty = false;
                });

                UpdateProjectDetails();
                UpdateProjectButtons();
            };

            addB.Click += (o, e) => {
                if (m_Project == null)
                    return;

                new NewAccount(this, AccountCreationCallback).Show();
            };

            togPremB.Click += (o, e) => {
                if (CurrentAccount == null)
                    return;

                CurrentAccount.SetPremium(!CurrentAccount.Premium);
                UpdateAccountContextInfo();
            };

            setNickB.Click += (o, e) => {
                if (CurrentAccount == null)
                    return;

                new NewAccount(this, AccountCreationCallbackEx, true, CurrentAccount.Username).Show();
            };

            foreach (MetroButton button in xtparentP.Controls.OfType<MetroButton>()) {
                if (button.Text != ">")
                    continue;

                button.Click += (o, e) => {
                    int idx = int.Parse(((string)((MetroButton)o).Tag));

                    if (idx >= 5) {
                        if (CurrentAccount == null)
                            return;

                        CurrentAccount.GetEquip(m_ContextualItems[idx], idx);
                    }

                    SetContextualView(m_ContextualItems[idx], idx);
                };
            }

            addCtxB.Click += (o, e) => ModifyCtx(true);
            remCtxB.Click += (o, e) => ModifyCtx(false);

            accP.Hide();
            xtctitemP.Hide();
            m_Accounts = new List<Account>();
            m_ClearBuffer = new List<MetroPanel>();
            m_IdleColor = Color.FromArgb(32, 32, 32);
            m_DownColor = Color.FromArgb(64, 64, 64);
            m_SelectedIndex = -1;
            m_ContextualItems = new List<string>[10]
            {
                Enum.GetNames(typeof(Account.Weapon)).ToList(),
                Enum.GetNames(typeof(Account.Skin)).ToList(),
                Enum.GetNames(typeof(Account.Hat)).ToList(),
                Enum.GetNames(typeof(Account.Perk)).ToList(),
                Enum.GetNames(typeof(Account.Item)).ToList(),
                new List<string>(),
                new List<string>(),
                new List<string>(),
                new List<string>(),
                new List<string>()
            };

            UpdateProjectDetails();
            UpdateProjectButtons();
            UpdateAccountPanels();
            UpdateAccountContextInfo();
            SetContextualView(null, -1);
        }

        void AccountCreationCallback(NewAccount.AccountCreationResult result, string username, string nickname) {
            if (result == NewAccount.AccountCreationResult.Cancel)
                return;

            m_Accounts.Add(Account.Create(username, nickname));
            UpdateAccountPanels();
            UpdateProjectButtons();
        }

        void AccountCreationCallbackEx(NewAccount.AccountCreationResult result, string username, string nickname) {
            if (result == NewAccount.AccountCreationResult.Cancel)
                return;

            CurrentAccount.SetNickname(nickname);
            UpdateAccountContextInfo();
            UpdateAccountPanels();
        }

        void SelectProject(string path) {
            foreach (string file in Directory.EnumerateFiles(path)) {
                if (file.Substring(file.LastIndexOf('\\') + 1) == Project.PROJECT_NAME) {
                    //found a project
                    UpdateProjectDetails(Project.ParseProject(path, out m_Project) ? "" : "Invalid project, repatch");
                }
            }

            if (m_Project == null) {
                //didnt find
                pathL.Text = path;
            }

            UpdateProjectButtons();
        }

        void UpdateProjectDetails(string error = "") {
            errorL.Text = error;
            errorL.Location = new Point(projectP.Location.X + projectP.Size.Width - errorL.Size.Width, errorL.Location.Y);
            pathL.Text = m_Project != null ? m_Project.Path : "-";
            patchL.Text = m_Project != null ? m_Project.PatchVersion.ToString() : "-";
            magicL.Text = m_Project != null ? m_Project.Magic : "-";
            dateL.Text = m_Project != null ? m_Project.Date.ToString() : "-";
        }

        public void UpdateProjectButtons() {
            selectB.Enabled = true;
            patchB.Enabled = (m_Project != null && (Project.PROJECT_PATCH_VERSION > m_Project.PatchVersion || m_Accounts.Count != m_Project.Accounts.Count))
                || (m_Project == null && pathL.Text != "-") || m_Accounts.Where(x => x.IsDirty).Count() > 0;
        }

        void SelectAccPanel(MetroPanel p, int idx) {
            foreach (MetroPanel panel in contaccP.Controls.OfType<MetroPanel>()) {
                panel.BackColor = panel == p ? m_DownColor : m_IdleColor;
            }
            m_SelectedIndex = idx;

            UpdateAccountContextInfo();
        }

        void UpdateAccountContextInfo() {
            xtuserL.Text = CurrentAccount != null ? CurrentAccount.Username : "-";
            xtnickL.Text = CurrentAccount != null ? CurrentAccount.Nickname : "-";
            xtpremiumL.Text = CurrentAccount != null ? CurrentAccount.Premium ? "Y" : "N" : "-";
        }

        void UpdateAccountPanels() {
            noaccL.Visible = m_Accounts.Count == 0;

            m_ClearBuffer.Clear();
            foreach (MetroPanel panel in contaccP.Controls.OfType<MetroPanel>()) {
                if (panel.Tag != null && ((string)panel.Tag) == "initial")
                    continue;

                m_ClearBuffer.Add(panel);
            }

            while (m_ClearBuffer.Count > 0) {
                int maxIdx = m_ClearBuffer.Count - 1;
                contaccP.Controls.Remove(m_ClearBuffer[maxIdx]);
                m_ClearBuffer.RemoveAt(maxIdx);
            }

            for (int i = 0; i < m_Accounts.Count; i++) {
                Account acc = m_Accounts[i];

                MetroPanel newPanel = CloneControl(accP);
                newPanel.Location = new Point(newPanel.Location.X, accP.Location.Y + i * accP.Size.Height);
                int localCopy = i;
                newPanel.Click += (o, e) => SelectAccPanel((MetroPanel)o, localCopy);

                contaccP.Controls.Add(newPanel);

                MetroLabel userLabel = CloneControl(useraccL);
                userLabel.Text = acc.Username;

                userLabel.Click += (o, e) => SelectAccPanel((MetroPanel)((MetroLabel)o).Parent, localCopy);
                newPanel.Controls.Add(userLabel);

                MetroLabel nickLabel = CloneControl(nickaccL);
                nickLabel.Text = acc.Nickname;

                nickLabel.Click += (o, e) => SelectAccPanel((MetroPanel)((MetroLabel)o).Parent, localCopy);
                newPanel.Controls.Add(nickLabel);
            }
        }

        void SetContextualView(List<string> items, int section) {
            m_ActiveSection = section;

            noitL.Visible = items == null || items.Count == 0 || section < 0;
            m_ClearBuffer.Clear();
            foreach (MetroPanel panel in contxtctP.Controls.OfType<MetroPanel>()) {
                if (panel.Tag != null && ((string)panel.Tag) == "initial")
                    continue;

                m_ClearBuffer.Add(panel);
            }

            while (m_ClearBuffer.Count > 0) {
                int maxIdx = m_ClearBuffer.Count - 1;
                contxtctP.Controls.Remove(m_ClearBuffer[maxIdx]);
                m_ClearBuffer.RemoveAt(maxIdx);
            }

            m_SelectedItem = null;

            xctxInfoL.Text = CurrentAccount != null ? CurrentAccount.GetCtxItems(section) : "";

            if (items == null)
                return;

            int offset = 0;

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                if (item == "None") {
                    offset--;
                    continue;
                }

                MetroPanel newPanel = CloneControl(xtctitemP);
                newPanel.Location = new Point(newPanel.Location.X, xtctitemP.Location.Y + (i + offset) * xtctitemP.Size.Height);
                newPanel.Click += (o, e) => ContextualCallback(newPanel, item, section);

                contxtctP.Controls.Add(newPanel);

                MetroLabel itemLabel = CloneControl(xtctitemL);
                itemLabel.Text = item;

                itemLabel.Click += (o, e) => ContextualCallback(newPanel, item, section);
                newPanel.Controls.Add(itemLabel);
            }
        }

        void ContextualCallback(Panel p, string item, int section) {
            System.Diagnostics.Debug.WriteLine($"CB {item}, {section}");

            if (m_SelectedCtxPanel != null)
                m_SelectedCtxPanel.BackColor = m_IdleColor;

            p.BackColor = m_DownColor;
            m_SelectedCtxPanel = p;

            m_SelectedItem = new SelectedItem {
                Item = item,
                Section = section
            };
        }

        void ModifyCtx(bool action) {
            if (CurrentAccount == null || m_SelectedItem == null)
                return;

            CurrentAccount.ModifyCtx(m_SelectedItem.Section, m_SelectedItem.Item, action);

            xctxInfoL.Text = CurrentAccount.GetCtxItems(m_ActiveSection);
        }

        MetroLabel CloneControl(MetroLabel source) {
            MetroLabel l = new MetroLabel();
            l.Text = source.Text;
            l.Style = source.Style;
            l.UseStyleColors = source.UseStyleColors;
            l.FontWeight = source.FontWeight;
            l.FontSize = source.FontSize;
            l.Size = source.Size;
            l.Location = source.Location;
            l.Theme = source.Theme;
            l.TextAlign = source.TextAlign;
            l.BackColor = source.BackColor;
            l.UseCustomBackColor = source.UseCustomBackColor;
            return l;
        }

        MetroPanel CloneControl(MetroPanel source) {
            MetroPanel l = new MetroPanel();
            l.Text = source.Text;
            l.Style = source.Style;
            l.UseStyleColors = source.UseStyleColors;
            l.Size = source.Size;
            l.Location = source.Location;
            l.Theme = source.Theme;
            l.BackColor = source.BackColor;
            l.UseCustomBackColor = source.UseCustomBackColor;
            return l;
        }
    }
}
