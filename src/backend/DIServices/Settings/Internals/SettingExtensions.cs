using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

namespace Vrh.DIServices.Settings.Internals
{
	/// <summary>
	/// A C# eszközökkel való, kódba helyezett definiciók feldolgozásának a segédműveletei
	/// </summary>
	internal static class SettingExtensions
	{
		/// <summary>
		/// Visszadja a beállítás definicióból a definicoó alapján képzett kulcsot, mint stringet
		/// </summary>
		/// <param name="settingDefinition"></param>
		/// <returns></returns>
		public static string GetSettingKey(this Type settingDefinition)
		{
			StringBuilder sb = new StringBuilder();
			Type type = settingDefinition;
			do
			{
				if (type.DeclaringType != null)
				{
					if (sb.Length > 0)
					{
						sb.Insert(0, ":", 1);
					}
					sb.Insert(0, type.Name, 1);
				}
				type = type.DeclaringType;
			} while (type != null);
			return sb.ToString();
		}

		/// <summary>
		/// Visszadja a beállításhoz tartozó definiált Description értéket 
		/// TODO: Multilanguage támogatás?
		/// </summary>
		/// <param name="settingDefinition">A definiciót hordozó típus</param>
		/// <returns>Megjegyzés szövege (beállítás leírása)</returns>
		public static string GetDescription(this Type settingDefinition)
		{
			string description = string.Empty;
			var attribute = settingDefinition.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
			if (attribute != null)
			{
				description = attribute.Description;
			}
			return description;
		}

		/// <summary>
		/// Gets the version of setting from setting defination.
		/// (If Version attribute presents, else gets null.)
		/// </summary>
		/// <param name="settingDefinition">The setting definition.</param>
		/// <returns></returns>
		public static string GetVersion(this Type settingDefinition)
		{
			string version = null;
			var attribute = settingDefinition.GetCustomAttributes<VersionAttribute>().FirstOrDefault();
			if (attribute != null)
			{
				version = attribute.Version;
			}
			return version;
		}

		/// <summary>
		/// Visszadja a beállításhoz definiált típust, ha nincs megadva, akkor string
		/// </summary>
		/// <param name="settingDefinition">A definiciót hordozó típus</param>
		/// <returns>A beállítás típusa</returns>
		public static Type GetSettingType(this Type settingDefinition)
		{
			var attribute = settingDefinition.GetCustomAttributes<SettingTypeAttribute>().FirstOrDefault();
			if (attribute != null)
			{
				return attribute.Type;
			}
			return typeof(string);
		}

		/// <summary>
		/// Visszadja a beállítás definicióból a defult értéket
		/// </summary>
		/// <param name="settingDefinition">A definiciót hordozó típus</param>
		/// <returns>alapértelmezett érték</returns>
		public static T GetDefaultValue<T>(this Type settingDefinition)
		{
			var attribute = settingDefinition.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault();
			if (attribute != null)
			{
				return (T)attribute.DefaultValue;
			}
			if (typeof(T).IsEnum)
			{
				var enumValues = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
				foreach (var enumValue in enumValues)
				{
					var isDefaultAttribute = enumValue.GetCustomAttribute<IsDefaultAttribute>();
					if (isDefaultAttribute != null)
					{
						var ev = Enum.Parse(typeof(T), enumValue.Name);
						return (T)ev;
					}
				}
				return default;
			}
			else
			{
				var getter = settingDefinition.GetMethod("GetDefaultValue");
				if (getter != null)
				{
					if (getter.IsStatic)
					{
						return (T)getter.Invoke(null, null);
					}
					else
					{
						return (T)getter.Invoke(Activator.CreateInstance(settingDefinition, null), null);
					}
				}
				else
				{
					return default;
				}
			}
		}

		/// <summary>
		/// Visszadja a definició alapján a lehetséges értékeket, ha a beállítás egy választéklista értékeit veheti fel
		/// </summary>
		/// <param name="settingDefinition"></param>
		/// <returns></returns>
		public static List<SettingSelection> GetSettingSelections(this Type settingDefinition)
		{
			var settingSelections = new List<SettingSelection>();
			var attribute = settingDefinition.GetCustomAttributes<SettingSelectionsAttribute>().FirstOrDefault();
			if (attribute != null)
			{
				settingSelections = attribute.SettingSelections;
			}
			return settingSelections;
		}

		/// <summary>
		/// Visszadja a használandó module nevét a beállítás definicióból
		/// </summary>
		/// <param name="settingDefinition"></param>
		/// <returns></returns>
		public static string GetModuleName(this Type settingDefinition)
		{
			var currentType = settingDefinition;
			while (currentType.DeclaringType != null)
			{
				currentType = currentType.DeclaringType;
			}
			var attribute = currentType.GetCustomAttributes<ModuleKeyAttribute>().FirstOrDefault();
			return attribute?.ModuleKey;
		}

		/// <summary>
		/// Gets the all defined setting types under this holder type
		/// </summary>
		/// <param name="settingHolderType">Type of the setting holder.</param>
		/// <returns></returns>
		public static IEnumerable<SettingTypesWithKey> GetAllDefinedSettingType(this Type settingHolderType)
		{
			return GetAllNestedTypes(settingHolderType);			
		}

		/// <summary>
		/// Convert valueses to pretty format.
		/// </summary>
		/// <param name="storeFormat">Setting with the store formated values.</param>
		public static void ValuesToPrettyFormat(this SettingRepresentation storeFormat)
		{
			if (!storeFormat.PrettyFormat)
			{
				switch (storeFormat.DataType)
				{
					case DataType.String:
						storeFormat.DefaultValue = JsonConvert.DeserializeObject<string>(storeFormat.DefaultValue);
						storeFormat.DbSideValue = JsonConvert.DeserializeObject<string>(storeFormat.DbSideValue);
						storeFormat.CacheSideValue = JsonConvert.DeserializeObject<string>(storeFormat.CacheSideValue);
						break;
					case DataType.Complex:
						storeFormat.DefaultValue = SettingExtensions.JsonPrettify(storeFormat.DefaultValue);
						storeFormat.DbSideValue = SettingExtensions.JsonPrettify(storeFormat.DbSideValue);
						storeFormat.CacheSideValue = SettingExtensions.JsonPrettify(storeFormat.CacheSideValue);
						break;
					default:
						break;
				}
				storeFormat.PrettyFormat = true;
			}
		}

		/// <summary>
		/// Convert valueses to store format.
		/// </summary>
		/// <param name="prettyFormat">Setting with the pretty formated values.</param>
		public static void ValuesToStoreFormat(this SettingRepresentation prettyFormat)
		{
			if (prettyFormat.PrettyFormat)
			{
				switch (prettyFormat.DataType)
				{
					case DataType.String:
						prettyFormat.DefaultValue = JsonConvert.SerializeObject(prettyFormat.DefaultValue);
						prettyFormat.DbSideValue = JsonConvert.SerializeObject(prettyFormat.DbSideValue);
						prettyFormat.CacheSideValue = JsonConvert.SerializeObject(prettyFormat.CacheSideValue);
						break;
					case DataType.Complex:
						prettyFormat.DefaultValue = SettingExtensions.JsonUnPrettify(prettyFormat.DefaultValue);
						prettyFormat.DbSideValue = SettingExtensions.JsonUnPrettify(prettyFormat.DbSideValue);
						prettyFormat.CacheSideValue = SettingExtensions.JsonUnPrettify(prettyFormat.CacheSideValue);
						break;
					default:
						prettyFormat.DefaultValue = prettyFormat.DefaultValue.Replace("\"", "");
						prettyFormat.DbSideValue = prettyFormat.DbSideValue.Replace("\"", "");
						prettyFormat.CacheSideValue = prettyFormat.CacheSideValue.Replace("\"", "");
						break;
				}
				prettyFormat.PrettyFormat = false;
			}
		}

		/// <summary>
		/// This setting is sensitive data?
		/// Password, etc...
		/// </summary>
		/// <param name="settingType">Type of the setting.</param>
		/// <returns></returns>
		public static bool Sensitive(this Type settingType)
		{
			return settingType.GetCustomAttribute<SensitiveData>() != null;
		}

		/// <summary>
		/// Converts JSON string to pretty format.
		/// </summary>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		private static string JsonPrettify(string json)
		{
			using (var stringReader = new StringReader(json))
			using (var stringWriter = new StringWriter())
			{
				var jsonReader = new JsonTextReader(stringReader);
				var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
				jsonWriter.WriteToken(jsonReader);
				return stringWriter.ToString();
			}
		}

		/// <summary>
		/// Converts JSON string to store format
		/// </summary>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		private static string JsonUnPrettify(string json)
		{
			using (var stringReader = new StringReader(json))
			using (var stringWriter = new StringWriter())
			{
				var jsonReader = new JsonTextReader(stringReader);
				var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.None };
				jsonWriter.WriteToken(jsonReader);
				return stringWriter.ToString();
			}
		}

		/// <summary>
		/// Gets all nested types with setting key under currentType defination
		/// </summary>
		/// <param name="currentType">Type of the current.</param>
		/// <returns></returns>
		private static IEnumerable<SettingTypesWithKey> GetAllNestedTypes(Type currentType)
		{
			foreach (var nestedType in currentType.GetNestedTypes())
			{
				yield return new SettingTypesWithKey()
				{
					Key = nestedType.GetSettingKey(),
					SettingType = nestedType,
				};
				foreach (var nestesTypesUnderThisNestedType in GetAllNestedTypes(nestedType))
				{
					yield return nestesTypesUnderThisNestedType;
				}
			}
		}
	}
}
