
namespace WoWs_GameParams_Dumper
{
    partial class Form_Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip_Display = new System.Windows.Forms.ToolTip(this.components);
            this.label_Status = new System.Windows.Forms.Label();
            this.timer_PR = new System.Windows.Forms.Timer(this.components);
            this.menuStrip_Main = new System.Windows.Forms.MenuStrip();
            this.loadGPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateShipListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_Status
            // 
            this.label_Status.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label_Status.BackColor = System.Drawing.Color.Silver;
            this.label_Status.Location = new System.Drawing.Point(444, 3);
            this.label_Status.Margin = new System.Windows.Forms.Padding(4);
            this.label_Status.Name = "label_Status";
            this.label_Status.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_Status.Size = new System.Drawing.Size(256, 16);
            this.label_Status.TabIndex = 0;
            this.label_Status.Text = "Status: Idle";
            this.label_Status.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer_PR
            // 
            this.timer_PR.Interval = 1000;
            // 
            // menuStrip_Main
            // 
            this.menuStrip_Main.BackColor = System.Drawing.Color.Silver;
            this.menuStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadGPToolStripMenuItem,
            this.saveJSONToolStripMenuItem,
            this.loadJSONToolStripMenuItem,
            this.updateShipListToolStripMenuItem});
            this.menuStrip_Main.Location = new System.Drawing.Point(0, 0);
            this.menuStrip_Main.Name = "menuStrip_Main";
            this.menuStrip_Main.ShowItemToolTips = true;
            this.menuStrip_Main.Size = new System.Drawing.Size(704, 24);
            this.menuStrip_Main.TabIndex = 1;
            this.menuStrip_Main.Text = "menuStrip_Main";
            // 
            // loadGPToolStripMenuItem
            // 
            this.loadGPToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.loadGPToolStripMenuItem.Name = "loadGPToolStripMenuItem";
            this.loadGPToolStripMenuItem.ShowShortcutKeys = false;
            this.loadGPToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.loadGPToolStripMenuItem.Text = "Load GP";
            this.loadGPToolStripMenuItem.ToolTipText = "Unpickles and Loads GameParams.data";
            this.loadGPToolStripMenuItem.Click += new System.EventHandler(this.LoadGPToolStripMenuItem_Click);
            // 
            // saveJSONToolStripMenuItem
            // 
            this.saveJSONToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveJSONToolStripMenuItem.Enabled = false;
            this.saveJSONToolStripMenuItem.Name = "saveJSONToolStripMenuItem";
            this.saveJSONToolStripMenuItem.ShowShortcutKeys = false;
            this.saveJSONToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.saveJSONToolStripMenuItem.Text = "Save JSON";
            this.saveJSONToolStripMenuItem.ToolTipText = "Save GameParams to JSON";
            this.saveJSONToolStripMenuItem.Click += new System.EventHandler(this.SaveJSONToolStripMenuItem_Click);
            // 
            // loadJSONToolStripMenuItem
            // 
            this.loadJSONToolStripMenuItem.Name = "loadJSONToolStripMenuItem";
            this.loadJSONToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.loadJSONToolStripMenuItem.Text = "Load JSON";
            this.loadJSONToolStripMenuItem.Click += new System.EventHandler(this.LoadJSONToolStripMenuItem_Click);
            // 
            // updateShipListToolStripMenuItem
            // 
            this.updateShipListToolStripMenuItem.Margin = new System.Windows.Forms.Padding(100, 0, 0, 0);
            this.updateShipListToolStripMenuItem.Name = "updateShipListToolStripMenuItem";
            this.updateShipListToolStripMenuItem.Size = new System.Drawing.Size(104, 20);
            this.updateShipListToolStripMenuItem.Text = "Update Ship List";
            this.updateShipListToolStripMenuItem.Click += new System.EventHandler(this.UpdateShipListToolStripMenuItem_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 281);
            this.Controls.Add(this.label_Status);
            this.Controls.Add(this.menuStrip_Main);
            this.MainMenuStrip = this.menuStrip_Main;
            this.MaximizeBox = false;
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WoWs GameParams Dumper";
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.menuStrip_Main.ResumeLayout(false);
            this.menuStrip_Main.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip_Display;
        private System.Windows.Forms.Label label_Status;
        private System.Windows.Forms.Timer timer_PR;
        private System.Windows.Forms.MenuStrip menuStrip_Main;
        private System.Windows.Forms.ToolStripMenuItem loadGPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateShipListToolStripMenuItem;
    }
}

