using UnityEngine;

public class ChefSystems : Feature
{
    public ChefSystems(Contexts contexts, GameObject chefPrefab)
    {
        Add(new CreateChefSystem(contexts, chefPrefab));
    }
}
