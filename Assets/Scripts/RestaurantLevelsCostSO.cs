using System;
using UnityEngine;

[Serializable]
public struct RestaurantLevel
{
    public GameObject prefab;
    public int upgradeCost;
}

[CreateAssetMenu()]
public class RestaurantLevelsCostSO : ScriptableObject
{
    public RestaurantLevel[] restaurantLevels;
}
