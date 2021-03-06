﻿using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

namespace ExermonDevManager.Core.Data {

	/// <summary>
	/// 数据控制器
	/// </summary>
	public static class DataLoader {

		/// <summary>
		/// 更新时间格式
		/// </summary>
		// 系统内部存储的日期格式
		public const string SystemDateFormat = "yyyy-MM-dd";
		public const string SystemDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

		// 显示的日期格式
		public const string DisplayDateFormat = "yyyy 年 MM 月 dd 日";
		public const string DisplayDateTimeFormat = "yyyy 年 MM 月 dd 日 HH:mm:ss";

		/// <summary>
		/// 判断JsonData是否包含键
		/// </summary>
		/// <param name="json">要判断的JsonData</param>
		/// <param name="key">键</param>
		/// <returns>是否包含</returns>
		public static bool contains(JsonData json, string key, bool showLog = false) {
			bool res = json.Keys.Contains(key);
			//if (!res && showLog)
			//	Debug.LogWarning("Hasn't Key: " + key + " in " + json.ToJson());
			return res;
		}

		#region 加载JsonData

		/// <summary>
		/// 加载数据
		/// </summary>
		/// <param name="json">数据</param>
		/// <param name="key">键</param>
		public static JsonData load(JsonData json, string key, bool returnNull = true) {
			if (contains(json, key, true)) return json[key];
			if (returnNull) return null;
			var data = new JsonData();
			data.SetJsonType(JsonType.Object);
			return data;
		}

		/// <typeparam name="T">读取数据类型</typeparam>
		/// <param name="val">原始值</param>
		/// <param name="ignoreNull">是否忽略空值</param>
		public static T load<T>(T val, JsonData json, string key, bool ignoreNull = false) {
			if (!contains(json, key, true)) return val; // 如果不存在键，则返回原样
			return (T)load(typeof(T), val, json[key], ignoreNull);
		}
		/// <param name="data">数据</param>
		public static T load<T>(T val, JsonData data, bool ignoreNull = false) {
			if (data == null)
				// 如果值为空且忽略空值，则返回原样
				return ignoreNull ? val : default;
			return (T)load(typeof(T), val, data);
		}
		/// <param name="type">类型</param>
		public static object load(Type type, object val, JsonData json, string key, bool ignoreNull = false) {
			if (!contains(json, key, true)) return val; // 如果不存在键，则返回原样
			return load(type, val, json[key], ignoreNull);
		}
		public static object load(Type type, object val, JsonData data, bool ignoreNull = false) {
			if (data == null)
				// 如果值为空且忽略空值，则返回原样
				return ignoreNull ? val : default;
			// 判断特殊类型
			try {
				if (type.IsSubclassOf(typeof(BaseData)) || type == typeof(BaseData)) {
					// 只有当值为空（默认）时才会创建对象
					if (val == default) val = Activator.CreateInstance(type);
					((BaseData)val).load(data);
					return val;
				} else return _load(type, data);
			} catch (Exception e) {
				//Debug.LogError(e + "\n" + e.StackTrace + "\n\nError (load type: " + type + ", ori val: " + val + ")\n" +
				//	" in " + data.ToJson());
				throw new Exception();
			}
		}
		public static T load<T>(JsonData json, string key) {
			if (!contains(json, key, true)) return default;
			try {
				return (T)_load(typeof(T), json[key]);
			} catch (Exception e) {
				//Debug.LogError(e + "\n" + e.StackTrace + "\n\nError (load type: " + typeof(T) + ")\n" +
				//	" in " + key + " of " + json.ToJson());
				throw new Exception();
			}
		}
		public static T load<T>(JsonData data) {
			if (data == null) return default;
			try {
				return (T)_load(typeof(T), data);
			} catch (Exception e) {
				//Debug.LogError(e + "\n" + e.StackTrace + "\n\nError (load type: " + typeof(T) + ")\n" +
				//	" in " + data.ToJson());
				throw new Exception();
			}
		}
		public static object load(Type type, JsonData json, string key) {
			if (!contains(json, key, true)) return default;
			try {
				return _load(type, json[key]);
			} catch (Exception e) {
				//Debug.LogError(e + "\n" + e.StackTrace + "\n\nError (load type: " + type + ")\n" +
				//	" in " + key + " of " + json.ToJson());
				throw new Exception();
			}
		}
		public static object load(Type type, JsonData data) {
			try {
				return _load(type, data);
			} catch (Exception e) {
				//Debug.LogError(e + "\n" + e.StackTrace + "\n\nError (load type: " + type + ")\n" +
				//	" in " + data.ToJson());
				throw new Exception();
			}
		}
		private static object _load(Type type, JsonData data) {
			if (data == null) return default;

			// 处理数组情况
			if (type.IsArray) {
				if (!data.IsArray) return default;
				var cnt = data.Count;
				var eleType = type.GetElementType();
				var res = Array.CreateInstance(eleType, cnt);
				for (var i = 0; i < cnt; ++i)
					res.SetValue(load(eleType, data[i]), i);
				return res;
			}

			// 处理列表情况
			var interfaces = type.GetInterfaces();
			foreach (var interf in interfaces)
				if (interf.Name == typeof(ICollection<>).Name) {
					if (!data.IsArray) return default;
					var cnt = data.Count;
					var eleType = type.GetGenericArguments()[0];
					var res = Activator.CreateInstance(type);
					for (var i = 0; i < cnt; ++i)
						type.GetMethod("Add").Invoke(res,
							new object[] { load(eleType, data[i]) });
					return res;
				}
			//if (type.Name == typeof(List<>).Name) {
			//}

			// 处理特殊类型
			if (type == typeof(DateTime)) return loadDateTime(data);
			if (type == typeof(TimeSpan)) return loadTimeSpan(data);
			if (type == typeof(Tuple<int, string>)) return loadTuple(data);

			if (type.IsSubclassOf(typeof(BaseData)) || type == typeof(BaseData))
				return loadData(type, data);

			// 处理基本类型
			if (type == typeof(int)) return (int)data;
			if (type == typeof(double))
				return data.IsDouble ? (double)data : (int)data;
			if (type == typeof(float))
				return data.IsDouble ? (float)(double)data : (int)data;
			if (type == typeof(string)) return (string)data;
			if (type == typeof(bool)) return (bool)data;

			// 其他情况下，直接返回即可
			return data;
		}

		#region 具体加载函数

		/// <summary>
		/// 加载日期时间
		/// </summary>
		/// <param name="json">数据</param>
		/// <returns>加载的字符串</returns>
		static DateTime loadDateTime(JsonData data) {
			try { return Convert.ToDateTime((string)data); } catch { return default; }
		}

		/// <summary>
		/// 加载TimeSpan
		/// </summary>
		/// <param name="json">数据</param>
		/// <returns>加载的TimeSpan</returns>
		static TimeSpan loadTimeSpan(JsonData data) {
			return new TimeSpan((int)data);
		}

		/// <summary>
		/// 加载(int, string)二元组
		/// </summary>
		/// <param name="json">数据</param>
		/// <returns>加载的二元组</returns>
		static Tuple<int, string> loadTuple(JsonData data) {
			return new Tuple<int, string>((int)data[0], (string)data[1]);
		}

		/// <summary>
		/// 加载数据
		/// </summary>
		/// <param name="type">类型</param>
		/// <param name="data">数据</param>
		static BaseData loadData(Type type, JsonData data) {
			return BaseData.create(type, data);
			//var res = (BaseData)Activator.CreateInstance(type);
			//res.load(data); return res;
		}

		#endregion

		#endregion

		#region 转化为JsonData

		/// <summary>
		/// 转化为JsonData
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="data">数据</param>
		/// <param name="format">转化格式</param>
		/// <returns>JsonData</returns>
		public static JsonData convert<T>(T data, string format = "") {
			return convert(typeof(T), data, format);
		}
		/// <param name="type">类型</param>
		public static JsonData convert(Type type, object data, string format = "") {
			if (data == null) return null;
			try {

				format = format.ToLower();
				bool isArray = false;
				Type eleType = null;

				// 处理数组情况
				if (isArray = type.IsArray)
					eleType = type.GetElementType();
				//else if (isArray = (type.Name == typeof(List<>).Name))
				//    eleType = type.GetGenericArguments()[0];
				else {
					// 处理列表情况
					var interfaces = type.GetInterfaces();
					foreach (var interf in interfaces)
						if (interf.Name == typeof(ICollection<>).Name) {
							eleType = type.GetGenericArguments()[0];
							isArray = true; break;
						}
				}

				if (isArray) {
					var array = (IEnumerable)data;
					var json = new JsonData();
					json.SetJsonType(JsonType.Array);
					foreach (var d in array)
						json.Add(convert(eleType, d, format));
					return json;
				}

				// 处理特殊类型
				if (type == typeof(DateTime) && format == "date") return convertDate((DateTime)data);
				if (type == typeof(DateTime)) return convertDateTime((DateTime)data);
				if (type == typeof(Tuple<int, string>)) return convertTuple((Tuple<int, string>)data);

				if (type.IsSubclassOf(typeof(BaseData)) || type == typeof(BaseData))
					return convertData(data as BaseData);

				// 处理基本类型
				if (type == typeof(int)) return (int)data;
				if (type == typeof(double)) return (double)data;
				if (type == typeof(float)) return (double)data;
				if (type == typeof(string)) return (string)data;
				if (type == typeof(bool)) return (bool)data;

				// 其他情况下，直接返回即可
				return data as JsonData;
			} catch (Exception e) {
				//Debug.LogError(e + "\n" + e.StackTrace + "\n\nError (convert type: "
				//	+ type + ") in covnerting " + data);
				throw new Exception();
			}
		}

		#region 具体转化函数

		/// <summary>
		/// 转化日期时间
		/// </summary>
		/// <param name="data">日期时间</param>
		/// <returns>转化后的JsonData</returns>
		static JsonData convertDateTime(DateTime data) {
			if (data == null) return "";
			return data.ToString(SystemDateTimeFormat);
		}

		/// <summary>
		/// 转化日期时间
		/// </summary>
		/// <param name="data">日期时间</param>
		/// <returns>转化后的JsonData</returns>
		static JsonData convertDate(DateTime data) {
			if (data == null) return "";
			return data.ToString(SystemDateFormat);
		}

		/// <summary>
		/// 转化(int, string)二元组
		/// </summary>
		/// <param name="data">二元组数据</param>
		/// <returns>转化后的JsonData</returns>
		static JsonData convertTuple(Tuple<int, string> data) {
			var json = new JsonData();
			json.SetJsonType(JsonType.Array);
			if (data == null) return json;
			json.Add(data.Item1); json.Add(data.Item2);
			return json;
		}

		/// <summary>
		/// 转化数据
		/// </summary>
		/// <typeparam name="T">要转化的类型</typeparam>
		/// <param name="data">数据</param>
		public static JsonData convertData(BaseData data) {
			if (data != null) return data.toJson();
			var json = new JsonData();
			json.SetJsonType(JsonType.Object);
			return json;
		}

		#endregion

		#region 特殊格式转化

		/// <summary>
		/// 转化四舍五入的浮点数
		/// </summary>
		/// <param name="x">浮点数</param>
		/// <param name="d">保留小数</param>
		/// <param name="intRes">整数结果</param>
		/// <returns></returns>
		public static JsonData convertDouble(
			double x, bool intRes = false, int d = 2) {
			if (intRes) return (int)x;
			x = Math.Round(x, d);
			var m = Math.Pow(10, d);
			// 如果 x 很接近 0（小数点 d 位以内为 0），则置为 0
			if ((int)(x * m) == 0) return 0;
			// 否则直接返回
			return x;
			//return (!intRes && x == (int)x ? (int)x : x);
		}

		#endregion

		#endregion

		#region 其他工具
		/*
        /// <summary>
        /// 类型转化并判断
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>如果可以转换，返回转换后对象，否则返回 default</returns>
        public static bool castPredicate<T>(object obj, Predicate<T> p) {
            if (obj == default) return default;
            Type ot = obj.GetType(), nt = typeof(T);
            if (ot == nt || ot.IsSubclassOf(nt)) return p((T)obj);
            return false;
        }
        
        /// <summary>
        /// 类型转化
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>如果可以转换，返回转换后对象，否则返回 default</returns>
        public static T cast<T>(object obj) {
            if (obj == default) return default;
            Type ot = obj.GetType(), nt = typeof(T);
            if (ot == nt || ot.IsSubclassOf(nt)) return (T)obj;
            return default;
        }
        */
		/// <summary>
		/// 下划线命名法转化为小驼峰命名法
		/// </summary>
		/// <param name="str">待转化字符串</param>
		/// <param name="spliter">下划线字符</param>
		/// <returns>转化结果</returns>
		public static string underline2LowerHump(string str, char spliter = '_') {
			string res = ""; bool flag = false;
			for (int i = 0; i < str.Length; i++) {
				if (i == 0) res += char.ToLower(str[i]);
				else if (str[i] == spliter) flag = true;
				else if (flag) {
					res += char.ToUpper(str[i]);
					flag = false;
				} else res += str[i];
			}
			return res;
		}

		/// <summary>
		/// 下划线命名法转化为大驼峰命名法
		/// </summary>
		/// <param name="str">待转化字符串</param>
		/// <param name="spliter">下划线字符</param>
		/// <returns>转化结果</returns>
		public static string underline2UpperHump(string str, char spliter = '_') {
			string res = ""; bool flag = true;
			for (int i = 0; i < str.Length; i++) {
				if (str[i] == spliter) flag = true;
				else if (flag) {
					res += char.ToUpper(str[i]);
					flag = false;
				} else res += str[i];
			}
			return res;
		}

		/// <summary>
		/// 驼峰命名法转化为下划线命名法
		/// </summary>
		/// <param name="str">待转化字符串</param>
		/// <param name="spliter">下划线字符</param>
		/// <returns>转化结果</returns>
		public static string hump2Underline(string str, char spliter = '_') {
			string res = "";
			for (int i = 0; i < str.Length; i++) {
				if (i > 0 && char.IsUpper(str[i])) {
					res += spliter;
					res += char.ToLower(str[i]);
				} else res += str[i];
			}
			return res;
		}

		#endregion
	}
}