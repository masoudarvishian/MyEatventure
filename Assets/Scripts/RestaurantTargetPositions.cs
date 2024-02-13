using UnityEngine;

public class RestaurantTargetPositions : MonoBehaviour
{
    [SerializeField] private Transform[] customerSpots;
    [SerializeField] private Transform[] kitchenSpots;

    public Transform GetFirstCustomerSpot() => customerSpots[0];

    public Transform GetFirstKitchenSpot() => kitchenSpots[0];

    private void Start()
    {
        var entity = Contexts.sharedInstance.game.CreateEntity();
        entity.AddWaitingCustomer(GetFirstCustomerSpot().position);
        entity.isPreparingOrder = false;
        entity.AddDelivered(false);
    }
}
