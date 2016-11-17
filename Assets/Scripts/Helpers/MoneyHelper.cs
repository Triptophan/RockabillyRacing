
public static class MoneyHelper
{
	public static void MakePurchase(int cost)
	{
		PlayerData data = new PlayerData();

		if (data.PlayerCash < cost) return;

		data.PlayerCash -= cost;
	}
}