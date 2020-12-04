namespace MRK
{
    partial class NewAccount
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.createB = new MetroFramework.Controls.MetroButton();
            this.cancelB = new MetroFramework.Controls.MetroButton();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.userTB = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.nickTB = new MetroFramework.Controls.MetroTextBox();
            this.SuspendLayout();
            // 
            // createB
            // 
            this.createB.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.createB.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.createB.Location = new System.Drawing.Point(399, 159);
            this.createB.Name = "createB";
            this.createB.Size = new System.Drawing.Size(370, 51);
            this.createB.TabIndex = 3;
            this.createB.Text = "Create";
            this.createB.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.createB.UseSelectable = true;
            // 
            // cancelB
            // 
            this.cancelB.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.cancelB.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.cancelB.Location = new System.Drawing.Point(23, 159);
            this.cancelB.Name = "cancelB";
            this.cancelB.Size = new System.Drawing.Size(370, 51);
            this.cancelB.TabIndex = 3;
            this.cancelB.Text = "Cancel";
            this.cancelB.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.cancelB.UseSelectable = true;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel1.Location = new System.Drawing.Point(137, 60);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(91, 25);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel1.TabIndex = 4;
            this.metroLabel1.Text = "Username";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // userTB
            // 
            // 
            // 
            // 
            this.userTB.CustomButton.Image = null;
            this.userTB.CustomButton.Location = new System.Drawing.Point(355, 1);
            this.userTB.CustomButton.Name = "";
            this.userTB.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.userTB.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.userTB.CustomButton.TabIndex = 1;
            this.userTB.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.userTB.CustomButton.UseSelectable = true;
            this.userTB.CustomButton.Visible = false;
            this.userTB.Lines = new string[0];
            this.userTB.Location = new System.Drawing.Point(241, 61);
            this.userTB.MaxLength = 32767;
            this.userTB.Name = "userTB";
            this.userTB.PasswordChar = '\0';
            this.userTB.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.userTB.SelectedText = "";
            this.userTB.SelectionLength = 0;
            this.userTB.SelectionStart = 0;
            this.userTB.ShortcutsEnabled = true;
            this.userTB.Size = new System.Drawing.Size(377, 23);
            this.userTB.TabIndex = 5;
            this.userTB.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.userTB.UseSelectable = true;
            this.userTB.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.userTB.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel2.Location = new System.Drawing.Point(137, 100);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(90, 25);
            this.metroLabel2.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel2.TabIndex = 4;
            this.metroLabel2.Text = "Nickname";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // nickTB
            // 
            // 
            // 
            // 
            this.nickTB.CustomButton.Image = null;
            this.nickTB.CustomButton.Location = new System.Drawing.Point(355, 1);
            this.nickTB.CustomButton.Name = "";
            this.nickTB.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.nickTB.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.nickTB.CustomButton.TabIndex = 1;
            this.nickTB.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.nickTB.CustomButton.UseSelectable = true;
            this.nickTB.CustomButton.Visible = false;
            this.nickTB.Lines = new string[0];
            this.nickTB.Location = new System.Drawing.Point(241, 101);
            this.nickTB.MaxLength = 32767;
            this.nickTB.Name = "nickTB";
            this.nickTB.PasswordChar = '\0';
            this.nickTB.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.nickTB.SelectedText = "";
            this.nickTB.SelectionLength = 0;
            this.nickTB.SelectionStart = 0;
            this.nickTB.ShortcutsEnabled = true;
            this.nickTB.Size = new System.Drawing.Size(377, 23);
            this.nickTB.TabIndex = 5;
            this.nickTB.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.nickTB.UseSelectable = true;
            this.nickTB.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.nickTB.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // NewAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 232);
            this.ControlBox = false;
            this.Controls.Add(this.nickTB);
            this.Controls.Add(this.userTB);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.cancelB);
            this.Controls.Add(this.createB);
            this.Name = "NewAccount";
            this.Resizable = false;
            this.Text = "Create account";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroButton createB;
        private MetroFramework.Controls.MetroButton cancelB;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroTextBox userTB;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroTextBox nickTB;
    }
}