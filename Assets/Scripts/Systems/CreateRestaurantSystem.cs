using Entitas;
using Entitas.Unity;
using System.Linq;
using UniRx;
using UnityEngine;

public sealed class CreateRestaurantSystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly RestaurantLevelsCostSO _restaurantLevelsCost;

    public CreateRestaurantSystem(Contexts contexts, RestaurantLevelsCostSO restaurantLevelsCost)
    {
        _contexts = contexts;
        _restaurantLevelsCost = restaurantLevelsCost;
    }

    public void Initialize()
    {
        var currentRestaurantLevel = GetRepositoryEntity().currentRestaurantLevel.value;
        var restaurantObj = GameObject.Instantiate(_restaurantLevelsCost.restaurantLevels[currentRestaurantLevel].prefab);
        restaurantObj.transform.position = Vector3.zero;

        var e = _contexts.game.CreateEntity();
        e.isRestaurant = true;
        e.AddVisual(restaurantObj);
        restaurantObj.Link(e);
    }

    private GameEntity GetRepositoryEntity() => _contexts.game.GetGroup(GameMatcher.Coin).GetEntities().First();
}
