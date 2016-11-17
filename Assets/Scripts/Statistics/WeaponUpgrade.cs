using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponUpgrade<T> where T : struct, IConvertible
{
	private List<Upgrade<T>> _maxUpgradeLevels;

	public T Type { get; private set; }

	public List<Upgrade<T>> MaxUpgradeLevels
	{
		get
		{
			if (_maxUpgradeLevels == null) _maxUpgradeLevels = new List<Upgrade<T>>();

			return _maxUpgradeLevels;
		}
		set
		{
			_maxUpgradeLevels = value;
		}
	}

	public WeaponUpgrade(T type)
	{
		if (!typeof(T).IsEnum)
		{
			throw new ArgumentException("T must be an enumerated type");
		}
		Type = type;
		MaxUpgradeLevels = new List<Upgrade<T>>();

		T[] values = (T[])System.Enum.GetValues(typeof(T));
		for (int i = 0; i < values.Length; i++)
		{
			Upgrade<T> item = new Upgrade<T>((T)values[i], 0);
			MaxUpgradeLevels.Add(item);
		}
	}
}