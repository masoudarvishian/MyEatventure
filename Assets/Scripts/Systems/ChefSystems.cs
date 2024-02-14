using UnityEngine;

public class ChefSystems : Feature
{
    public ChefSystems(Contexts contexts, GameObject chefPrefab, CooldownHelper cooldownHelper, RestaurantTargetPositions restaurantTargetPositions, 
        GameObject customersPrefab, GameObject customersParent, Transform customerSpawnPoint)
    {
        Add(new FrontDeskSystem(contexts, restaurantTargetPositions));
        Add(new CreateChefSystem(contexts, chefPrefab));
        Add(new CreateCustomerSystem(contexts, customersPrefab, customersParent, customerSpawnPoint));
        Add(new MovingChefSystem(contexts));
        //Add(new TargetPositionEmitterSystem(contexts, restaurantTargetPositions));
        Add(new SetChefAsMoverSystem(contexts));
        //Add(new CleanUpTargetPositionSystem(contexts));
        Add(new DeliveryOrderSystem(contexts));
        Add(new TakingOrderDetectorSystem(contexts));
        Add(new CooldownSystem(contexts, cooldownHelper));
        Add(new PreparingOrderDetectorSystem(contexts, restaurantTargetPositions));
        Add(new StartCookingSystem(contexts, restaurantTargetPositions));
        Add(new ReadyOrderDetectorSystem(contexts, restaurantTargetPositions));
        Add(new SetCustomerAsMoverSystem(contexts, restaurantTargetPositions));
    }
}
