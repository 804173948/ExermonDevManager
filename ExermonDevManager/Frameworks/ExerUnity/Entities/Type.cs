﻿using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

using System.ComponentModel.DataAnnotations.Schema;

namespace ExermonDevManager.Frameworks.ExerUnity.Entities {

	using Core.Data;
	using Core.Utils;
	using Core.CodeGen;
	using Core.Managers;
	
	/// <summary>
	/// 类型类
	/// </summary>
	public abstract class Type_ : CoreEntity {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		[ControlField("代码", 10)]
		public string code { get; set; } = "";
		[AutoConvert]
		[ControlField("可继承", 20)]
		public bool derivable { get; set; } = true;

		/// <summary>
		/// 构造函数
		/// </summary>
		public Type_() { }
		public Type_(string name, string code = null,
			string description = "", bool buildIn = true) :
			base(name, description, buildIn) {
			this.code = code ?? name; derivable = !buildIn;
		}
		
		///// <summary>
		///// 继承的类型
		///// </summary>
		///// <returns></returns>
		//public virtual List<T> inheritTypes<T>() where T : Type_ {
		//	var res = new List<T>(inherits.Count);
		//	foreach (var id in inherits)
		//		res.Add(poolGet<T>(id));
		//	return res;
		//}

		///// <summary>
		///// 派生的类型
		///// </summary>
		///// <returns></returns>
		//public virtual List<T> deriveTypes<T>() where T : Type_ {
		//	var res = new List<T>();
		//	var types = poolGet<T>();

		//	foreach (var type in types) {
		//		var inherits = type.inheritTypes<T>();
		//		if (inherits != null && inherits.Contains(this as T))
		//			res.Add(type);
		//	}

		//	return res;
		//}
	}

	/// <summary>
	/// 类型类
	/// </summary>
	public abstract partial class Type_<P> : Type_ where P : Param {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		[ControlField("字段", 5)]
		[InverseProperty("ownerType")]
		public List<P> params_ { get; protected set; } = new List<P>();

		/// <summary>
		/// 构造函数
		/// </summary>
		public Type_() { }
		public Type_(string name, string code = null,
			string description = "", bool buildIn = true) :
			base(name, code, description, buildIn) { }
	}

	/// <summary>
	/// 类型类
	/// </summary>
	public abstract partial class Type_<T, P> : Type_<P> 
		where T: Type_<T, P> where P : Param {

		/// <summary>
		/// 继承关系
		/// </summary>
		public class InheritDerive : CoreEntity {

			/// <summary>
			/// 属性
			/// </summary>
			[AutoConvert]
			public int deriveTypeId { get; set; }
			public T deriveType { get; set; }

			[AutoConvert]
			public int inheritTypeId { get; set; }
			[ControlField("基类")]
			public T inheritType { get; set; }

			/// <summary>
			/// 不显示的字段
			/// </summary>
			/// <returns></returns>
			protected static new string[] listExclude() {
				return new string[] { "name", "description", "buildIn" };
			}

		}

		/// <summary>
		/// 继承/派生类型（关联属性）
		/// </summary>
		[AutoConvert]
		[ControlField("继承", 10)]
		[InverseProperty("deriveType")]
		public List<InheritDerive> inherits { get; } = new List<InheritDerive>();
		[AutoConvert]
		//[ControlField("派生", 10)]
		[InverseProperty("inheritType")]
		public List<InheritDerive> derives { get; } = new List<InheritDerive>();

		/// <summary>
		/// 构造函数
		/// </summary>
		public Type_() { }
		public Type_(string name, string code = null,
			string description = "", bool buildIn = true) :
			base(name, code, description, buildIn) { }

		/// <summary>
		/// 总属性
		/// </summary>
		/// <returns></returns>
		protected CacheAttr<List<P>> totalParams_ = null;
		protected List<P> _totalParams_() {
			var res = new List<P>(params_);

			foreach (var inherit in inherits)
				res.AddRange(inherit.inheritType.totalParams());

			return res;
		}
		public List<P> totalParams() {
			return totalParams_?.value();
		}

		///// <summary>
		///// 继承的类型
		///// </summary>
		///// <returns></returns>
		//protected CacheAttr<List<T>> inheritTypes_ = null;
		//protected List<T> _inheritTypes_() {
		//	var res = new List<T>(inherits.Count);
		//	foreach (var id in inherits)
		//		res.Add(poolGet<T>(id));
		//	return res;
		//}
		//public List<T> inheritTypes() {
		//	return inheritTypes_?.value();
		//}
		//public override List<T2> inheritTypes<T2>() {
		//	if (typeof(T2) == typeof(T))
		//		return inheritTypes() as List<T2>;
		//	return base.inheritTypes<T2>();
		//}
		public List<T> inheritTypes() {
			var res = new List<T>();

			foreach (var inherit in inherits)
				res.Add(inherit.inheritType);

			return res;
		}

		/// <summary>
		/// 派生的类型
		/// </summary>
		/// <returns></returns>
		public List<T> deriveTypes() {
			var res = new List<T>();

			foreach (var derive in derives)
				res.Add(derive.deriveType);

			return res;
		}
		//public override List<T2> deriveTypes<T2>() {
		//	if (typeof(T2) == typeof(T))
		//		return deriveTypes() as List<T2>;
		//	return base.deriveTypes<T2>();
		//}
	}
}