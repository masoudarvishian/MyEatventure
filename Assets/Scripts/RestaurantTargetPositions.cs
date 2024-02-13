using UnityEngine;

public class RestaurantTargetPositions : MonoBehaviour
{
    [SerializeField] private Transform[] customerSpots;
    [SerializeField] private Transform[] kitchenSpots;

    public Transform GetFirstCustomerSpot()
    {
        return customerSpots[0];
    }

    private void Start()
    {
        var entity = Contexts.sharedInstance.game.CreateEntity();
        entity.AddWaitingCustomer(GetFirstCustomerSpot().position);
    }
}
