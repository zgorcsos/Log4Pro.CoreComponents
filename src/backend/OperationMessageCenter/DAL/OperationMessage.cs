using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Log4Pro.CoreComponents.OperationMessageCenter.DAL
{
    /// <summary>
    /// Represent an operation message 
    /// </summary>
    [Index(nameof(Module))]
    [Index(nameof(Instance))]
    [Index(nameof(OtherFilter))]
    [Index(nameof(TimeStamp))]
    [Index(nameof(MessageCategory))]
    [Index(nameof(Handled))]
    [Index(nameof(HandledTimeStamp))]
    [Index(nameof(Thread))]
    [Table(nameof(OperationMessageCenterContext.OperationMessages), Schema = OperationMessageCenterContext.DB_SCHEMA)]
    public class OperationMessage
	{
		/// <summary>
		/// PK
		/// </summary>
		[Key]
		public int Id { get; set; }

        /// <summary>
        /// Id of module 
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Module { get; set; }

        /// <summary>
        /// Instance or user id
        /// </summary>
        [MaxLength(100)]
        public string Instance { get; set; }

        /// <summary>
        /// Egyéb érték, amit az üzenet megjelenítés/kezeléssel kapcsolatban lehet felhasználni konkrét alklamzásfüggően pl. felhasználókhóz, csoportokhoz kötve
        /// </summary>
        [MaxLength(100)]
        public string OtherFilter { get; set; }

        /// <summary>
        /// Üzenet időbélyege
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Üzenet kategóriája
        /// </summary>
        public MessageCategory MessageCategory { get; set; }
        
        /// <summary>
        /// Üzenet
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Ez a flag jelzi, hogy az üzenet kezelt/megtekintett
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? HandledTimeStamp { get; set; }

        /// <summary>
        /// Az üzenetet jóváhagyó felhasználó
        /// </summary>
        [MaxLength(100)]
        public string HandledBy { get; set; }

        /// <summary>
        /// Ez egy olyan mező, amely segítségével összefoghatunk egy adott művelet közben keletkező üzeneteket
        /// Pl.: valaminek a feldolgozása
        /// </summary>
        [MaxLength(100)]
        public string Thread { get; set; }

        /// <summary>
        /// NP: Az üzenethez tartozó további adatok
        /// </summary>
        public virtual List<AdditionalMessageData> AdditionalMessageDatas { get; set; }
    }
}
