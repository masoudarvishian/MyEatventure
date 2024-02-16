using System;
using UnityEngine;

[Serializable]
public struct CoinLevel
{
    public int coin;
    public int upgradeCost;
}

[CreateAssetMenu()]
public class DrinkCoinLevelsPriceSO : ScriptableObject
{
    public CoinLevel[] coinLevels;
}
