using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.Classes.Graphes
{
	/// <summary>
	/// A node in a graph
	/// </summary>
	public class Node
	{
		/// <summary>
		/// The key (uniqu identifier under parent node).
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// The child node.
		/// </summary>
		public Node Child { get; set; }
	}
}
