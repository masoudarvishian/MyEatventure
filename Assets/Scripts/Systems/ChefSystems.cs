﻿using UnityEngine;

public class ChefSystems : Feature
{
    public ChefSystems(Contexts contexts, GameObject chefPrefab, CooldownHelper cooldownHelper, RestaurantTargetPositions restaurantTargetPositions)
    {
        Add(new CreateChefSystem(contexts, chefPrefab));
        Add(new MovingChefSystem(contexts));
        Add(new TargetPositionEmitterSystem(contexts));
        Add(new SetChefAsMoverSystem(contexts));
        //Add(new CleanUpTargetPositionSystem(contexts));
        Add(new TakingOrderDetectorSystem(contexts));
        Add(new CooldownSystem(contexts, cooldownHelper));
        Add(new PreparingOrderDetectorSystem(contexts, restaurantTargetPositions));
        Add(new StartCookingSystem(contexts, restaurantTargetPositions));
    }
}
