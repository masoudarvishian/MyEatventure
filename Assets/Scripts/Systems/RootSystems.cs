using UnityEngine;

public class RootSystems : Feature
{
    public RootSystems(
        Contexts contexts, 
        GameObject chefPrefab,
        GameObject customersPrefab, 
        GameObject customersParent, 
        Transform customerSpawnPoint, 
        Transform customerLeavingPoint,
        DummyUI dummyUI, 
        DrinkCoinLevelsPriceSO drinkCoinLevelsPrice,
        RestaurantLevelsCostSO restaurantLevelsCost)
    {
        Add(new RepositorySystem(contexts, drinkCoinLevelsPrice, restaurantLevelsCost, dummyUI));
        Add(new CreateRestaurantSystem(contexts, restaurantLevelsCost));
        Add(new FrontDeskSystem(contexts));
        Add(new KitchenSystem(contexts));
        Add(new CreateChefSystem(contexts, chefPrefab));
        Add(new CreateCustomerSystem(contexts, customersPrefab, customersParent, customerSpawnPoint));
        Add(new MovingChefSystem(contexts));
        Add(new DeliveryOrderSystem(contexts));
        Add(new TakingOrderSystem(contexts));
        Add(new StartCookingSystem(contexts));
        Add(new CustomerUISystem(contexts));
        Add(new MoveCustomerSystem(contexts, customerLeavingPoint));
        Add(new AssignChefCustomerSystem(contexts));
        Add(new DummyUISystem(contexts, dummyUI, drinkCoinLevelsPrice, restaurantLevelsCost));
        Add(new ChefUISystem(contexts));
    }
}
