﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExermonDevManager.Scripts.Controls {

	using Data;

	public partial class ExerNumericUpDown : NumericUpDown, IExerControl {
		public ExerNumericUpDown() {
			InitializeComponent();
		}

		/// <summary>
		/// 注册更新事件
		/// </summary>
		/// <param name="event_"></param>
		public void registerUpdateEvent(EventHandler event_) {
			ValueChanged += event_;
		}

		/// <summary>
		/// 绑定数据
		/// </summary>
		/// <param name="data"></param>
		public virtual void bind(BaseData data) {
			DataBindings.Clear();
			DataBindings.Add("Value", data, Name, false, 
				DataSourceUpdateMode.OnPropertyChanged);
		}
		
		/// <summary>
		/// 更新
		/// </summary>
		public virtual void update() { }
	}
}