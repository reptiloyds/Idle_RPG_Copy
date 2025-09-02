namespace PleasantlyGames.RPG.Runtime.InAppPurchase.Delegates
{
    public delegate void PurchaseDelegate(string productId, bool success);
    public delegate void ConsumeDelegate(string productId, bool success);
}