﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExermonDevManager.Core {

	using Forms;
	using Entities;

	using Managers;

	using Utils;

	/// <summary>
	/// 框架接口
	/// </summary>
	public interface IFramework {

		/// <summary>
		/// 框架名称
		/// </summary>
		string name { get; }

		/// <summary>
		/// 实体类型
		/// </summary>
		Type[] entityTypes { get; }

	}

	/// <summary>
	/// 框架类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Framework<T> : Singleton<T>, 
		IFramework where T : Framework<T>, new() {

		/// <summary>
		/// 框架层类型
		/// </summary>
		public enum LayerType {
			Entities, Forms, Controls, Default, Config
		}

		/// <summary>
		/// 框架名称
		/// </summary>
		public virtual string name => GetType().Name;

		/// <summary>
		/// 实体类型
		/// </summary>
		public virtual Type[] entityTypes => getLayerTypes(LayerType.Entities, typeof(BaseEntity));
		public virtual Type[] formTypes => getLayerTypes(LayerType.Forms, typeof(ExerForm<>));

		/// <summary>
		/// 初始化
		/// </summary>
		public Framework() {
			EntitiesManager.registerEntities(this, entityTypes);
			ExerFormManager.registerForms(formTypes);
			TemplateManager.registerFramework(this);
		}

		#region 反射获取数据

		/// <summary>
		/// 获取层的类型
		/// </summary>
		/// <param name="layerName"></param>
		/// <returns></returns>
		Type[] getLayerTypes(LayerType layer, Type tType) {
			var type = GetType();
			var name = type.Namespace + "." + layer.ToString();
			return ReflectionUtils.getNamespaceTypes(
				type.Assembly, name, tType);
		}

		#endregion
	}

	/// <summary>
	/// 核心框架
	/// </summary>
	public class CoreFramework : Framework<CoreFramework> {

		/// <summary>
		/// 模板名称
		/// </summary>
		public override string name => "Core";
	}

	/// <summary>
	/// 框架管理类
	/// </summary>
	public static class FrameworkManager {

		/// <summary>
		/// 框架
		/// </summary>
		public static List<IFramework> frameworks = new List<IFramework>();

		/// <summary>
		/// 初始化标记
		/// </summary>
		static bool initialized = false;

		/// <summary>
		/// 初始化
		/// </summary>
		public static void initialize() {
			if (initialized) return;
			initialized = true;

			initializeFrameworks();
		}

		/// <summary>
		/// 初始化所有框架
		/// </summary>
		static void initializeFrameworks() {
			Console.WriteLine("initializeFrameworks");

			var assem = typeof(FrameworkManager).Assembly;
			var types = ReflectionUtils.getNamespaceTypes(
				assem, parent: typeof(IFramework));

			foreach (var type in types) {
				Console.WriteLine("Initializing framework: " + type);

				var getFunc = type.GetMethod("Get",
					ReflectionUtils.DefaultStaticFlag);
				var val = getFunc.Invoke(null, null);

				frameworks.Add(val as IFramework);
			}
		}
	}

}
