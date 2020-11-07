﻿namespace ExermonDevManager.Forms {
	partial class MainForm2 {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.dataView = new ExermonDevManager.Scripts.Controls.ExerDataGridView();
			this.exermon_managerDataSet = new ExermonDevManager.exermon_managerDataSet();
			this.tableCombox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.saveData = new System.Windows.Forms.Button();
			this.tableSetting = new System.Windows.Forms.Button();
			this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
			((System.ComponentModel.ISupportInitialize)(this.dataView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.exermon_managerDataSet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// dataView
			// 
			this.dataView.AllowUserToOrderColumns = true;
			this.dataView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			this.dataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataView.Location = new System.Drawing.Point(12, 43);
			this.dataView.Name = "dataView";
			this.dataView.RowTemplate.Height = 23;
			this.dataView.Size = new System.Drawing.Size(737, 395);
			this.dataView.TabIndex = 0;
			// 
			// exermon_managerDataSet
			// 
			this.exermon_managerDataSet.DataSetName = "exermon_managerDataSet";
			this.exermon_managerDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// tableCombox
			// 
			this.tableCombox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.tableCombox.FormattingEnabled = true;
			this.tableCombox.Location = new System.Drawing.Point(57, 16);
			this.tableCombox.Name = "tableCombox";
			this.tableCombox.Size = new System.Drawing.Size(530, 20);
			this.tableCombox.TabIndex = 1;
			this.tableCombox.SelectedIndexChanged += new System.EventHandler(this.tableCombox_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "选择表";
			// 
			// saveData
			// 
			this.saveData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.saveData.Location = new System.Drawing.Point(593, 14);
			this.saveData.Name = "saveData";
			this.saveData.Size = new System.Drawing.Size(75, 23);
			this.saveData.TabIndex = 3;
			this.saveData.Text = "保存数据";
			this.saveData.UseVisualStyleBackColor = true;
			this.saveData.Click += new System.EventHandler(this.saveData_Click);
			// 
			// tableSetting
			// 
			this.tableSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tableSetting.Location = new System.Drawing.Point(674, 14);
			this.tableSetting.Name = "tableSetting";
			this.tableSetting.Size = new System.Drawing.Size(75, 23);
			this.tableSetting.TabIndex = 4;
			this.tableSetting.Text = "表配置";
			this.tableSetting.UseVisualStyleBackColor = true;
			// 
			// MainForm2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(761, 450);
			this.Controls.Add(this.dataView);
			this.Controls.Add(this.tableSetting);
			this.Controls.Add(this.saveData);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tableCombox);
			this.Name = "MainForm2";
			this.Text = "艾瑟萌开发管理系统2.0";
			this.Load += new System.EventHandler(this.TestForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.exermon_managerDataSet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Scripts.Controls.ExerDataGridView dataView;
		private System.Windows.Forms.ComboBox tableCombox;
		private System.Windows.Forms.Label label1;
		private exermon_managerDataSet exermon_managerDataSet;
		private System.Windows.Forms.Button saveData;
		private System.Windows.Forms.Button tableSetting;
		private System.Windows.Forms.BindingSource bindingSource;
	}
}