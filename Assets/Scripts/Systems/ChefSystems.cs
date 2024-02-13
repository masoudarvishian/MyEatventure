﻿using UnityEngine;

public class ChefSystems : Feature
{
    public ChefSystems(Contexts contexts, GameObject chefPrefab)
    {
        Add(new CreateChefSystem(contexts, chefPrefab));
        Add(new MovingChefSystem(contexts));
        Add(new TargetPositionEmitterSystem(contexts));
        Add(new SetChefAsMoverSystem(contexts));
        Add(new CleanUpTargetPositionSystem(contexts));
    }
}