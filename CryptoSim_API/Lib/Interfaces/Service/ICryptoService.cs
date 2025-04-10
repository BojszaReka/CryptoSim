
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.Interfaces.ServiceInterfaces
{
	public interface ICryptoService
	{
		Task DecreaseCryptoQuantity(string v, int quantity);
		Task<bool> doesCryptoExists(string v);
		Task<Crypto> GetCrypto(string v);
		Task IncreaseCryptoQuantity(string v, int quantity);
	}
}
