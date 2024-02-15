using TMPro;
using UnityEngine;

public class RootSystems : Feature
{
    public RootSystems(
        Contexts contexts, 
        GameObject chefPrefab,
        RestaurantTargetPositions restaurantTargetPositions, 
        GameObject customersPrefab, 
        GameObject customersParent, 
        Transform customerSpawnPoint, 
        Transform customerLeavingPoint, 
        TMP_Text coinText, 
        DrinkCoinLevelsPriceSO drinkCoinLevelsPrice)
    {
        Add(new FrontDeskSystem(contexts, restaurantTargetPositions));
        Add(new CreateChefSystem(contexts, chefPrefab));
        Add(new CreateCustomerSystem(contexts, customersPrefab, customersParent, customerSpawnPoint, restaurantTargetPositions));
        Add(new MovingChefSystem(contexts));
        Add(new DeliveryOrderSystem(contexts));
        Add(new RepositorySystem(contexts, coinText, drinkCoinLevelsPrice));
        Add(new TakingOrderDetectorSystem(contexts, restaurantTargetPositions));
        Add(new StartCookingSystem(contexts, restaurantTargetPositions));
        Add(new CustomerUIPopupSystem(contexts));
        Add(new MoveCustomerSystem(contexts, customerLeavingPoint));
        Add(new AssignChefCustomerSystem(contexts));
    }
}
