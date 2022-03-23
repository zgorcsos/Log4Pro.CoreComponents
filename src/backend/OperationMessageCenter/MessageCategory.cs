using Log4Pro.CoreComponents.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.OperationMessageCenter
{
	/// <summary>
	/// Defined operation message category
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum MessageCategory
	{
		/// <summary>
		/// Fatális hiba 
		/// </summary>
		[Title("Végzetes hibák")]
		[Description("Az alapvető működést megakadályozó hibák kerülnek csak feljegyzésre.")]
		Fatal,
		/// <summary>
		/// Hiba
		/// </summary>
		[Title("Hibák")]
		[Description("Minden keletkező hiba feljegyzésre kerül.")]
		Error,
		/// <summary>
		/// Figyelmeztetés
		/// </summary>
		[Title("Figyelmeztetések")]
		[Description("Minden hiba és figyelmeztetés feljegyzésre kerül.")]
		Warning,
		/// <summary>
		/// Informális üzenet a működésről
		/// </summary>
		[Title("Működéi információk")]
		[Description("Minden fő működési információ, hiba és figyelmeztetés feljegyzésre kerül.")]
		Information,
		/// <summary>
		/// Részletes működési információ
		/// </summary>
		[Title("Részletes információk")]
		[Description("Minden rendelkezésre álló információ, hiba és figyelmeztetés feljegyzésre kerül.")]
		DetailInformation,
	}
}
