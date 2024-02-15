using System;
using UnityEngine;

[Serializable]
public struct CoinLevel
{
    public int level;
    public int coin;
    public int upgradePrice;
}

[CreateAssetMenu()]
public class CoinLevelsPriceSO : ScriptableObject
{
    public CoinLevel[] coinLevels;
}
