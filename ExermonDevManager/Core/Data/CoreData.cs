﻿using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

using System.ComponentModel;

namespace ExermonDevManager.Core.Data {
	using LitJson;
	using Utils;
	using CodeGen;

	/// <summary>
	/// 可转化为控件数据的数据
	/// </summary>
	public abstract class CoreData : BaseData {

		/// <summary>
		/// 控件显示字段属性特性
		/// </summary>
		[AttributeUsage(AttributeTargets.Field |
			AttributeTargets.Property | AttributeTargets.Method)]
		public class ControlFieldAttribute : Attribute,
			IComparable<ControlFieldAttribute> {

			/// <summary>
			/// 字段名
			/// </summary>
			public string name;

			/// <summary>
			/// 优先级（越低越前）
			/// </summary>
			public int rank;

			/// <summary>
			/// 宽度
			/// </summary>
			public int width;

			/// <summary>
			/// 相关是成员信息
			/// </summary>
			public MemberInfo memberInfo;

			/// <summary>
			/// 构造函数
			/// </summary>
			public ControlFieldAttribute(string name, int rank = 0, int width = 128) {
				this.name = name; this.rank = rank; this.width = width;
			}

			/// <summary>
			/// 比较函数
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public int CompareTo(ControlFieldAttribute other) {
				return rank.CompareTo(other.rank);
			}
		}

		/// <summary>
		/// 字段数据
		/// </summary>
		public class FieldData : IComparable<FieldData> {

			/// <summary>
			/// 特性
			/// </summary>
			public ControlFieldAttribute attr;

			/// <summary>
			/// 值
			/// </summary>
			public string value;

			/// <summary>
			/// 构造函数
			/// </summary>
			public FieldData(ControlFieldAttribute attr, string value) {
				this.attr = attr; this.value = value;
			}

			/// <summary>
			/// 比较函数
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public int CompareTo(FieldData other) {
				return attr.CompareTo(other.attr);
			}
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		[ControlField("名称")]
		public string name { get; set; } = "";
		[AutoConvert]
		[ControlField("描述", 100)]
		public string description { get; set; } = "";

		/// <summary>
		/// 内建数据
		/// </summary>
		[ControlField("内建", 1000)]
		public bool buildIn { get; set; } = false;

		/// <summary>
		/// 构造函数
		/// </summary>
		public CoreData() { }
		public CoreData(string name, string description = "", bool buildIn = true) {
			this.name = name; this.description = description;
			this.buildIn = buildIn;
		}

		#region 配置

		/// <summary>
		/// 是否包含在列表控件中
		/// </summary>
		/// <returns></returns>
		public virtual bool isIncluded() {
			return true; // !buildIn;
		}

		/// <summary>
		/// 可以保存到文件
		/// </summary>
		/// <returns></returns>
		protected override bool isSaveEnable() {
			return !buildIn;
		}

		/// <summary>
		/// 下拉列表文本
		/// </summary>
		/// <returns></returns>
		public virtual string comboText() {
			return id + ". " + name;
		}

		/// <summary>
		/// 获取分组名称
		/// </summary>
		/// <returns></returns>
		public virtual string groupText() {
			return name;
		}

		/// <summary>
		/// 获取分组键值
		/// </summary>
		/// <returns></returns>
		public virtual string groupKey() {
			return null;
		}

		#endregion

		#region 列表数据

		/// <summary>
		/// 不显示的字段
		/// </summary>
		/// <returns></returns>
		protected static string[] listExclude() {
			return new string[] { "buildIn" };
		}

		/// <summary>
		/// 不显示的字段
		/// </summary>
		/// <returns></returns>
		protected static string[] listExclude(Type type) {
			var flag = BindingFlags.FlattenHierarchy |
				BindingFlags.Static | ReflectionUtils.DefaultFlag;
			var func = type.GetMethod("listExclude", flag, 
				null, new Type[] { }, null);
			return func.Invoke(null, null) as string[];
		}

		/// <summary>
		/// 获取用于显示的字段数据
		/// </summary>
		/// <returns></returns>
		public static List<ControlFieldAttribute> getFieldSettings(Type type) {
			var res = new List<ControlFieldAttribute>();
			//var exclude = type.InvokeMember("listExclude",
			//	ReflectionUtils.DefaultFlag, null, null, new object[0]) as string[];
			var exclude = listExclude(type);

			ReflectionUtils.processAttribute
				<MemberInfo, ControlFieldAttribute>(
				type, (m, attr) => {
					attr.memberInfo = m;
					if (!exclude.Contains(m.Name))
						res.Add(attr);
				}
			);

			res.Sort();

			return res;
		}

		/// <summary>
		/// 获取用于显示的字段数据
		/// </summary>
		/// <returns></returns>
		public List<FieldData> getFieldData() {
			var res = new List<FieldData>();
			var exclude = listExclude(GetType());

			PropertyInfo p; FieldInfo f; MethodInfo func;

			ReflectionUtils.processAttribute
				<MemberInfo, ControlFieldAttribute>(
				GetType(), (m, attr) => {
					if (exclude.Contains(m.Name)) return;
					string value = "";

					if ((p = m as PropertyInfo) != null)
						value = p.GetValue(this)?.ToString();

					else if ((f = m as FieldInfo) != null)
						value = f.GetValue(this)?.ToString();

					else if ((func = m as MethodInfo) != null)
						value = func.Invoke(this, null)?.ToString();

					res.Add(new FieldData(attr, value));
				});

			res.Sort();

			return res;
		}

		#endregion

		#region 代码生成
		
		/// <summary>
		/// 获取代码生成器
		/// </summary>
		/// <returns></returns>
		public CodeGenerator getGenerator(Enum name) {
			return getGenerateManager().getGenerator(this, name);
		}

		/// <summary>
		/// 获取指定/所有生成器
		/// </summary>
		/// <returns></returns>
		public List<CodeGenerator> getGenerators(params Enum[] names) {
			return getGenerateManager().getGenerators(this, names);
		}

		/// <summary>
		/// 生成代码
		/// </summary>
		/// <returns></returns>
		public string genCode(Enum name) {
			var generator = this.getGenerator(name);
			return generator?.generate();
		}

		/// <summary>
		/// 生成代码列表
		/// </summary>
		/// <returns></returns>
		public List<GeneratedCode> genCodes(Enum name) {
			var generator = this.getGenerator(name);
			generator?.generate();
			return generator?.codes;
		}

		/// <summary>
		/// 获取自身对应的生成管理类
		/// </summary>
		/// <returns></returns>
		IGenerateManager getGenerateManager() {
			return getGenerateManager(GetType());
		}

		/// <summary>
		/// 获取自身对应的生成管理类
		/// </summary>
		/// <returns></returns>
		public static IGenerateManager getGenerateManager(Type type) {
			var mType = typeof(GenerateManager<>).MakeGenericType(type);
			var getFunc = mType.GetMethod("Get", ReflectionUtils.DefaultFlag | 
				BindingFlags.Static | BindingFlags.FlattenHierarchy);
			return getFunc.Invoke(null, null) as IGenerateManager;
		}

		#endregion

	}
	
	/// <summary>
	/// 核心实体
	/// </summary>
	public abstract class CoreEntity : CoreData {

		/// <summary>
		/// 显示内容
		/// </summary>
		public string displayName => id + ". " + name;

		/// <summary>
		/// 是否支持ID
		/// </summary>
		/// <returns></returns>
		protected sealed override bool idEnable() {
			return true;
		}

		/// <summary>
		/// ID偏移量
		/// </summary>
		/// <returns></returns>
		protected override int idOffset() {
			return 1;
		}

		/// <summary>
		/// 获取属性
		/// </summary>
		/// <param name="propName"></param>
		/// <returns></returns>
		public object this[string propName] => getPropValue(propName);

		/// <summary>
		/// 获取属性值
		/// </summary>
		/// <param name="propName"></param>
		/// <returns></returns>
		public object getPropValue(string propName) {
			var prop = GetType().GetProperty(propName);
			return prop.GetValue(this);
		}

		/// <summary>
		/// 获取属性类型
		/// </summary>
		/// <param name="propName"></param>
		/// <returns></returns>
		public Type getPropType(string propName) {
			var prop = GetType().GetProperty(propName);
			return prop.PropertyType;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public CoreEntity() { }
		public CoreEntity(string name, string description = "", bool buildIn = true) :
			base(name, description, buildIn) { }

		#region 表数据

		/// <summary>
		/// 表名
		/// </summary>
		/// <returns></returns>
		public static string getTableName(Type type) {
			return getTableSetting(type).tableName;
		}

		/// <summary>
		/// 显示名
		/// </summary>
		/// <returns></returns>
		public static string getDisplayName(Type type) {
			return getTableSetting(type).displayName;
		}

		/// <summary>
		/// 获取表设置
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static TableSetting getTableSetting(Type type) {
			var attr = type.GetCustomAttribute<TableSetting>(true);
			return attr ?? new TableSetting(type);
		}

		#endregion
	}

}