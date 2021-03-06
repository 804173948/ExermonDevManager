﻿using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

using System.ComponentModel.DataAnnotations.Schema;

namespace ExermonDevManager.Frameworks.ExerUnity.Entities {

	using Core.Data;
	using Core.Entities;
	using Core.CodeGen;

	/// <summary>
	/// 继承关系
	/// </summary>
	[TableSetting("模型继承", false)]
	public class ModelInheritDerive : InheritDerive<Model> { }

	/// <summary>
	/// 模型类
	/// </summary>
	[TableSetting("模型")]
	[TemplateSetting("Models")]
	[TemplateSetting((int)GenType.Model, "Model", "模型代码")]
	[TemplateSetting((int)GenType.ModelProp, "ModelProp", "模型属性代码")]
	[TemplateSetting((int)GenType.ModelPropDeclare, "ModelPropDeclare", "模型属性声明代码")]
	public class Model : Type_<Model, ModelField, 
		ModelInheritDerive>, IModuleEntity {

		/// <summary>
		/// 生成代码类型
		/// </summary>
		public enum GenType {
			Model, ModelProp, ModelPropDeclare,
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int moduleId { get; set; }
		[ControlField("所属模块", 2)]
		public Module module { get; set; }
		
		[AutoConvert]
		[ControlField("是否抽象", 10)]
		public bool abstract_ { get; set; } = false; // 抽象类

		/// <summary>
		/// 构造函数
		/// </summary>
		public Model() { }
		public Model(string name, string code, bool buildIn = true) :
			base(name, code, buildIn) { }
		public Model(string name, bool buildIn = true) :
			base(name, buildIn) { }

		/// <summary>
		/// 获取分组键值
		/// </summary>
		/// <returns></returns>
		public override string groupKey() {
			return moduleId.ToString();
		}

		#region 代码生成

		#region 部分生成

		///// <summary>
		///// 生成前端代码
		///// </summary>
		///// <returns></returns>
		//public string fCode() {
		//	return genCode(GenType.ExermonModelProp);
		//}

		#endregion

		/// <summary>
		/// 模型代码
		/// </summary>
		public string moduleCode => module?.code;

		/// <summary>
		/// 继承代码
		/// </summary>
		/// <returns></returns>
		List<string> inheritCodes() {
			var inherits = inheritTypes();
			var res = new List<string>(inherits.Count);
			foreach (var inherit in inherits)
				res.Add(inherit.code);
			return res;
		}

		/// <summary>
		/// 继承代码
		/// </summary>
		/// <returns></returns>
		string inheritsCode() {
			var codes = inheritCodes();
			return string.Join(", ", codes);
		}

		#endregion
	}
	
}
