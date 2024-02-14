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
        foreach (var chefEntity in entities)
        {
            if (HasReachedToTargetPosition(chefEntity, _restaurantTargetPositions.GetFirstKitchenSpot().position))
                AddCooldownEntity(3f);
        }
    }

    protected override bool Filter(GameEntity entity) => !entity.hasTargetPosition && entity.isChef;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.TargetPosition).AnyOf(GameMatcher.Chef).Removed());

    private bool HasReachedToTargetPosition(GameEntity entity, Vector3 targetPosition) =>
        Vector3.Distance(entity.position.value, targetPosition) <= Mathf.Epsilon;

    private void AddCooldownEntity(float value)
    {
        var cooldownEntity = _contexts.game.CreateEntity();
        cooldownEntity.AddCooldown(value);
    }
}
