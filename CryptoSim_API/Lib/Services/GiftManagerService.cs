using CryptoSim_API.Lib.Interfaces.Service;
using CryptoSim_API.Lib.Interfaces.ServiceInterfaces;
using CryptoSim_Lib.Enums;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Services
{
	public class GiftManagerService : IGiftService
	{
		private readonly CryptoContext _dbContext;
		private readonly IMemoryCache _cache;
		private readonly IServiceProvider _serviceProvider;

		public GiftManagerService(CryptoContext dbContext, IMemoryCache cache, IServiceProvider serviceProvider)
		{
			_dbContext = dbContext;
			_cache = cache;
			_serviceProvider = serviceProvider;
		}

		public async Task<IQueryable<Gift>> getGiftsAsync()
		{
			var cachedGifts = _cache.Get("gifts");

			if (cachedGifts != null && !string.IsNullOrEmpty(cachedGifts.ToString()))
			{
				var gifts = JsonConvert.DeserializeObject<List<Gift>>(cachedGifts.ToString());
				return gifts.AsQueryable<Gift>();
			}
			var giftsfFromDb = await _dbContext.Gifts.OrderBy(c => c.Id).ToListAsync();
			var serializedData = JsonConvert.SerializeObject(giftsfFromDb);
			_cache.Set("gifts", serializedData);
			return _dbContext.Gifts.OrderBy(c => c.Id);
		}

		public async Task<string> createGift(GiftRequestDTO request)
		{
			var transaction = _dbContext.Database.BeginTransaction();
			try
			{
				var scope = _serviceProvider.CreateScope();
				var _cryptoManager = scope.ServiceProvider.GetRequiredService<ICryptoService>();
				var crypto = await _cryptoManager.GetCrypto(request.CryptoId.ToString());
				var price = crypto.CurrentPrice * request.Quantity;

				var gift = new Gift
				{
					Id = Guid.NewGuid(),
					SenderUserId = request.SenderUserId,
					ReceiverUserId = request.ReceiverUserId,
					Status = EGiftStatus.Pending,
					CryptoId = request.CryptoId,
					Quantity = request.Quantity,
					PriceAtGift = price
				};
				_dbContext.Gifts.Add(gift);
				_dbContext.SaveChanges();
				_cache.Remove("gifts");
				transaction.Commit();
				return $"Gift request placed successfully, with Id: {gift.Id}";
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
			finally
			{
				transaction.Dispose();
			}
		}

		public async Task<bool> doesGiftExists(string giftId)
		{
			var gifts = await getGiftsAsync();
			var gift = gifts.FirstOrDefault(g => g.Id.ToString().Equals(giftId));
			return gift != null;
		}

		public async Task<Gift> getGift(string giftId)
		{
			if(! await doesGiftExists(giftId))
			{
				throw new Exception("The gift with the provided ID does not exist");
			}
			var gifts = await getGiftsAsync();
			var gift = gifts.FirstOrDefault(g => g.Id.ToString().Equals(giftId));
			return gift;
		}

		public async Task UpdateGiftStatus(string giftId, EGiftStatus status)
		{
			var gift = await getGift(giftId);
			var transaction = _dbContext.Database.BeginTransaction();
			try
			{
				gift.Status = status;
				_dbContext.Gifts.Update(gift);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("gifts");
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
			finally
			{
				transaction.Dispose();
			}
		}

		public async Task<double> GetGiftedQuantity(string cryptoId, string userId)
		{
			var gifts = await getGiftsAsync();

			var userGifts = gifts.Where(g => g.ReceiverUserId.ToString().Equals(userId) && g.CryptoId.ToString().Equals(cryptoId) && g.Status == EGiftStatus.Accepted);

			double totalGiftedQuantity = userGifts.Sum(g => g.Quantity);
			return totalGiftedQuantity;
		}
	}
}
