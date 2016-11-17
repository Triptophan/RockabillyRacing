using System;

public class Upgrade<T> where T : struct, IConvertible
{
	public T Type { get; private set; }
	public int MaxLevel { get; set; }

	public Upgrade(T type, int maxLevel)
	{
		if (!typeof(T).IsEnum)
		{
			throw new ArgumentException("T must be an enumerated type");
		}
		Type = type;
		MaxLevel = maxLevel;
	}
}