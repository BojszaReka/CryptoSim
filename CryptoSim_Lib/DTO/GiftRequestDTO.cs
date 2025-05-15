using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
	public class GiftRequestDTO
	{
		/// <summary>
		/// A küldő felhasználó azonosítója
		/// </summary>
		[Required]
		public Guid SenderUserId { get; set; }

		/// <summary>
		/// A fogadó felhasználó azonosítója
		/// </summary>
		[Required]
		public Guid ReceiverUserId { get; set; }

		/// <summary>
		/// Az ajándékozott kriptovaluta azonosítója
		/// </summary>
		[Required]
		public Guid CryptoId { get; set; }

		/// <summary>
		/// Az ajándékozott mennyiség
		/// </summary>
		[Range(0.00000001, double.MaxValue)]
		public int Quantity { get; set; }
	}
}
