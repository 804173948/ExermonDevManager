﻿using System;

namespace ExermonDevManager.Core.CodeGen {

	using Data;
	using Managers;

	/// <summary>
	/// 模板使用配置
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TemplateSetting : Attribute {

		/// <summary>
		/// 类型
		/// </summary>
		public int type;

		/// <summary>
		/// 模板文件名（只需名称即可）
		/// </summary>
		public string name;

		/// <summary>
		/// 是否全局
		/// </summary>
		public bool isGlobal = false;

		/// <summary>
		/// 描述
		/// </summary>
		public string description;

		/// <summary>
		/// 构造函数
		/// </summary>
		public TemplateSetting(string name) {
			this.name = name; isGlobal = true;
		}
		public TemplateSetting(int type, string name, string description = "") {
			this.type = type; this.name = name;
			this.description = description;
		}

		/// <summary>
		/// 获取模板实例
		/// </summary>
		/// <returns></returns>
		public CodeTemplate getTemplate(Type entityType) {
			var framework = EntitiesManager.getFramework(entityType);
			return TemplateManager.getTemplate(framework, name);
		}

		//public TemplateSetting(Type enumType, int type, string description = "") {
		//	this.type = type;
		//	name = Enum.GetName(enumType, type);
		//	this.description = description;
		//}
	}

	/// <summary>
	/// 模板项
	/// </summary>
	public class TemplateItem : CoreData {

		/// <summary>
		/// 常量定义
		/// </summary>
		const string GlobalName = "Global";
		const string GlobalDescription = "用于实际生成代码的模板";

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		[ControlField("描述", 100)]
		public string description { get; protected set; } = "";
		[AutoConvert]
		public bool isGlobal { get; protected set; }
		[AutoConvert]
		public int type { get; protected set; }
		[AutoConvert]
		public int templateId { get; protected set; }

		/// <summary>
		/// 枚举类型
		/// </summary>
		//public Type enumType;

		/// <summary>
		/// 构造函数
		/// </summary>
		public TemplateItem() { }
		public TemplateItem(CodeTemplate template, string desc = GlobalDescription) {
			isGlobal = true;
			name = GlobalName;
			description = desc;
			templateId = template.id;
		}
		public TemplateItem(Enum type, CodeTemplate template, string desc = "") {
			name = type.ToString(); description = desc;
			this.type = type.GetHashCode();
			templateId = template.id;
		}
		public TemplateItem(TemplateSetting setting, Type dataType) {
			name = setting.name; type = setting.type;
			description = setting.description;
			isGlobal = setting.isGlobal;

			var template = setting.getTemplate(dataType);
			templateId = template.id;
		}

		/// <summary>
		/// 获取模板实例
		/// </summary>
		/// <returns></returns>
		protected CacheAttr<CodeTemplate> template_ = null;
		protected CodeTemplate _template_() {
			return DataManager.poolGet<CodeTemplate>(templateId);
		}
		public CodeTemplate template() {
			return template_?.value();
		}

		#region 显示项

		/// <summary>
		/// 不显示的字段
		/// </summary>
		/// <returns></returns>
		protected static new string[] listExclude() {
			return new string[] { "code", "buildIn" };
		}

		/// <summary>
		/// 模板路径
		/// </summary>
		/// <returns></returns>
		[ControlField("路径", 1)]
		public string templatePath() {
			return template()?.path;
		}

		#endregion

	}

}
