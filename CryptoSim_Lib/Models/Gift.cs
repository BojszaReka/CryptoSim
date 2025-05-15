using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoSim_Lib.Models
{
	[Table("Gifts")]
	public class Gift
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Küldő felhasználó azonosítója
		/// </summary>
		[Required]
		public Guid SenderUserId { get; set; }

		/// <summary>
		/// Fogadó felhasználó azonosítója
		/// </summary>
		[Required]
		public Guid ReceiverUserId { get; set; }

		/// <summary>
		/// Ajándékozott kriptovaluta azonosítója
		/// </summary>
		[Required]
		public Guid CryptoId { get; set; }

		/// <summary>
		/// Ajándékozott mennyiség
		/// </summary>
		[Range(0.00000001, double.MaxValue)]
		public double Quantity { get; set; }

		/// <summary>
		/// Árfolyam az ajándékozás időpontjában (piaci ár)
		/// </summary>
		[Range(0, double.MaxValue)]
		public double PriceAtGift { get; set; }

		/// <summary>
		/// Az ajándékozás dátuma
		/// </summary>
		public DateTime GiftDate { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Ajándék státusza: Pending (letét), Accepted, Rejected
		/// </summary>
		public EGiftStatus Status { get; set; } = EGiftStatus.Pending;

		// Ha szükséges, további navigációs tulajdonságok, pl. User vagy Crypto entitások:
		[JsonIgnore]
		public User? SenderUser { get; set; }
		[JsonIgnore]
		public User? ReceiverUser { get; set; }
		[JsonIgnore]
		public Crypto? Crypto { get; set; }
	}
}
