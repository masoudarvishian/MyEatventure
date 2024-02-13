using Entitas;
using System.Collections.Generic;
using UnityEngine;

public sealed class PreparingOrderDetectorSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;

    public PreparingOrderDetectorSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions) : base(contexts.game)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var chefEntity in _contexts.game.GetGroup(GameMatcher.Chef).GetEntities())
        {
            if (Vector3.Distance(chefEntity.position.value, _restaurantTargetPositions.GetFirstKitchenSpot().position) > Mathf.Epsilon)
            {
                var e = _contexts.game.CreateEntity();
                e.AddTargetPosition(_restaurantTargetPositions.GetFirstKitchenSpot().position);
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isEnabled == false;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Cooldown).Removed());
    }
}
