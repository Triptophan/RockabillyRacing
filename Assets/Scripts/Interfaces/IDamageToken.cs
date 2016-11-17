using UnityEngine;
using System.Collections;

public interface IDamageToken
{
	float DamagePerTick { get; set; }
	float Duration { get; set; }
}

