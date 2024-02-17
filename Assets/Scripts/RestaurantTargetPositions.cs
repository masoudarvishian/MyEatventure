using UnityEngine;

public class RestaurantTargetPositions : MonoBehaviour
{
    [SerializeField] private Transform[] behindDeskSpots;
    [SerializeField] private Transform[] frontDeskSpots;
    [SerializeField] private Transform[] kitchenSpots;

    public Transform[] GetKitchenSpots() => kitchenSpots;

    public Transform[] GetFrontDeskSpots() => frontDeskSpots;

    public Transform[] GetBehindDeskSpots() => behindDeskSpots;
}
