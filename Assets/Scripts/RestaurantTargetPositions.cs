using UnityEngine;

public class RestaurantTargetPositions : MonoBehaviour
{
    [SerializeField] private Transform[] behindDeskSpots;
    [SerializeField] private Transform[] frontDeskSpots;
    [SerializeField] private Transform[] kitchenSpots;

    public Transform GetFirstCustomerSpot() => behindDeskSpots[0];

    public Transform GetFirstKitchenSpot() => kitchenSpots[0];

    public Transform[] GetFrontDeskSpots() => frontDeskSpots;

    public Transform[] GetBehindDeskSpots() => behindDeskSpots;
}
