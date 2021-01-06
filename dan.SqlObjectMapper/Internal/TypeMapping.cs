using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace dan.SqlObjectMapper.Internal
{
	/// <summary>
	/// Parse the properties in a type recursively
	/// </summary>
	internal class TypeMapping
	{
		static Type enumerableType = typeof(IEnumerable);
		static Type stringType = typeof(string);
		Type type;
		Dictionary<string, TypeInfo> properties = new Dictionary<string, TypeInfo>();
		HashSet<Type> typeCheck = new HashSet<Type>(); // Avoid infinite recursion

		public TypeMapping(Type type)
		{
			this.type = type;
			createTypeMapping();
		}

		public static bool IsEnumerableType(Type t)
		{
			return enumerableType.IsAssignableFrom(t);
		}

		/// <summary>
		/// For debug purpose
		/// </summary>
		/// <returns></returns>
		public KeyValuePair<string, string>[] GetMapping()
		{
			return properties.Select(r => new KeyValuePair<string, string>(r.Key, (Nullable.GetUnderlyingType(r.Value.Property.PropertyType)?.Name ?? r.Value.Property.PropertyType.Name) + (r.Value.Nullable ? "?" : ""))).ToArray();
		}

		public TypeInfo FindByName(string name)
		{
			if (properties.ContainsKey(name))
				return properties[name];
			else
				return null;
		}

		void createTypeMapping()
		{
			createSubTypeMapping(type, "");
		}

		void createSubTypeMapping(Type subType, string propName)
		{
			var props = subType.GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);

			for (int i = 0; i < props.Length; i++)
			{
				var prop = props[i];
				if (prop.PropertyType.IsValueType || prop.PropertyType == stringType)
				{
					properties.Add(propName + prop.Name, new TypeInfo { Property = prop, Nullable = Nullable.GetUnderlyingType(prop.PropertyType) != null });
				}
				else
				{
					if (!enumerableType.IsAssignableFrom(prop.PropertyType) && !typeCheck.Contains(prop.PropertyType))
					{
						properties.Add(propName + prop.Name, new TypeInfo { Property = prop, Nullable = true });

						typeCheck.Add(prop.PropertyType);
						createSubTypeMapping(prop.PropertyType, propName + prop.Name + ".");
						typeCheck.Remove(prop.PropertyType);
					}
				}
			}
		}
	}
}
