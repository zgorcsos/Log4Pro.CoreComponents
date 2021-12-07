using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Classes.Graphes
{
	/// <summary>
	/// An infinite depth address of data
	/// </summary>
	public class GraphPath
	{
		/// <summary>
		/// Creates an DataId (address of data) from a Type hierarchy (Nested types structure)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static GraphPath GenericDataIdFactory<T>()
		{
			var dataId = new GraphPath();
			var type = typeof(T);
			dataId.Entry = new Node { Key = type.Name };
			while (type != null)
			{
				(dataId.Entry, type) = dataId.Entry.InterChainFromNestedTypeHierarchy(type);
			}
			return dataId;
		}

		/// <summary>
		/// Sets the new start entry of this adress.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public void NewStartEntry(string key)
		{
			Entry = new Node() { Key = key, Child = Entry };
		}

		/// <summary>
		/// The Entry is a Node in the address with maximum one child node.
		/// </summary>
		public Node Entry { get; set; }

		/// <summary>
		/// Converts to string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			var entry = this.Entry;
			while (entry != null)
			{
				if (sb.Length > 0)
				{
					sb.Append(":");
				}
				sb.Append(entry.Key);
				entry = entry.Child;
			}
			return sb.ToString();
		}
	}
}
