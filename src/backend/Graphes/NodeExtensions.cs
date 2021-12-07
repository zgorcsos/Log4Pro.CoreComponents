using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Classes.Graphes
{
	/// <summary>
	/// A node in a graph-principle addressing
	/// </summary>
	public static class NodeExtensions
	{
		/// <summary>
		/// Creates a connection between two node by nested type hierarchy.
		/// </summary>
		/// <param name="node">The current node in hierarrchy.</param>
		/// <param name="currentType">Type of the current element in nested type hierarchy.</param>
		/// <returns>
		/// This is a Trumple. 
		/// Node: It is the created parent node if this is not the top class in nested type hierarchy. And the current node if is it.
		/// Type: It is the Declaring type in the nested type hierarchy, if exists. And null if this is the top class.
		/// </returns>
		public static (Node, Type) InterChainFromNestedTypeHierarchy(this Node node, Type currentType)
		{
			if (currentType.IsNested)
			{
				var declaringType = currentType.DeclaringType;
				var parentNode = new Node { Key = declaringType.Name, Child = node };
				return (parentNode, declaringType);
			}
			else
			{
				return (node, null);
			}
		}
	}
}
