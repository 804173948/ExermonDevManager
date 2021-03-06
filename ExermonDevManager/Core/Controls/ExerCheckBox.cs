﻿using System;
using System.Windows.Forms;

namespace ExermonDevManager.Core.Controls {

	using Data;

	public partial class ExerCheckBox : CheckBox, IExerEditControl {
		public ExerCheckBox() {
			InitializeComponent();
		}

		/// <summary>
		/// 注册更新事件
		/// </summary>
		/// <param name="event_"></param>
		public void registerUpdateEvent(Action action) {
			Click += (_, __) => action();
			KeyUp += (_, __) => action();
			LostFocus += (_, __) => action();
			CheckedChanged += (_, __) => action();
		}
		
		/// <summary>
		/// 绑定数据
		/// </summary>
		/// <param name="data"></param>
		public virtual void bind(CoreData data) {
			DataBindings.Clear();
			DataBindings.Add("Checked", data, Name, false,
				DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
