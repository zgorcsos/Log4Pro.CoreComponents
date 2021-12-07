﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4Pro.CoreComponents.DIServices.OperationMessageCenter
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
		Fatal,
		/// <summary>
		/// Hiba
		/// </summary>
		Error,
		/// <summary>
		/// Figyelmeztetés
		/// </summary>
		Warning,
		/// <summary>
		/// Informális üzenet a működésről
		/// </summary>
		Information,
		/// <summary>
		/// Részletes működési információ
		/// </summary>
		DetailInformation,
	}
}