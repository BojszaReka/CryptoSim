using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSim_Lib.DTO
{
	public class TransactionFeeReportDTO
	{
		// Összesített díj minden tranzakcióra vonatkozóan
		public double TotalFee { get; set; }

		// Napi bontású díjak
		public List<DailyFeeSummaryDTO> DailyFees { get; set; } = new();

		// Egyedi tranzakciós bontás
		public List<TransactionFeeDetailDTO> TransactionDetails { get; set; } = new();
	}

	// Napi összesített díjak
	public class DailyFeeSummaryDTO
	{
		public DateTime Date { get; set; }
		public double TotalFee { get; set; }
	}

	// Egyedi tranzakciós részletek
	public class TransactionFeeDetailDTO
	{
		public Guid TransactionId { get; set; }
		public Guid UserId { get; set; }
		public Guid CryptoId { get; set; }
		public double Fee { get; set; }
		public DateTime Timestamp { get; set; }
	}

}
