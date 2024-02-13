using Entitas;
using System.Collections.Generic;
using UnityEngine;

internal class StartCookingSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;

    public StartCookingSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions) : base(contexts.game)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            if (Vector3.Distance(entity.position.value, _restaurantTargetPositions.GetFirstKitchenSpot().position) <= Mathf.Epsilon)
            {
                var cooldownEntity = _contexts.game.CreateEntity();
                cooldownEntity.AddCooldown(3f);
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return !entity.isMover && entity.isChef;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Mover).Removed());
    }
}
