﻿using System.Windows.Forms;

namespace ExermonDevManager.Scripts.Forms {
	
	using Controls;

	public class SubFormForBase : SubForm {

		/// <summary>
		/// 控件
		/// </summary>
		public override Button saveButton => null;
		public override ComboBox rootComboBox => null;
		public override ExerDataGridView dataGridView => null;
		public override BindingSource dataBindingSource => null;

		/// <summary>
		/// 构造函数
		/// </summary>
		public SubFormForBase() { }

	}
}
