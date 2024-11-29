namespace Models
{
    public class CoinAmount
    {
        public decimal Amount { get; set; }
        public string CoinName { get; set; } 
        public Guid WalletId { get; set; }
        
        public CoinAmount()
        {
        }
        
        public CoinAmount(decimal amount, string coinName, Guid walletId)
        {
            Amount = amount;
            CoinName = coinName;
            WalletId = walletId;
        }
    }
}