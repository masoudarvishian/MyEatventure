using Entitas;
using System.Collections.Generic;
using UnityEngine;

public sealed class PreparingOrderDetectorSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;
    private readonly IGroup<GameEntity> _chefGroup;

    public PreparingOrderDetectorSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions) : base(contexts.game)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
        _chefGroup = _contexts.game.GetGroup(GameMatcher.Chef);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in _chefGroup.GetEntities())
        {
            if (HasNotReachedToTargetPosition(chefEntity, _restaurantTargetPositions.GetFirstKitchenSpot().position))
                chefEntity.AddTargetPosition(_restaurantTargetPositions.GetFirstKitchenSpot().position);
        }
    }

    protected override bool Filter(GameEntity entity) => entity.isEnabled == false;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.Cooldown).Removed());

    private bool HasNotReachedToTargetPosition(GameEntity chefEntity, Vector3 targetPosition) =>
        Vector3.Distance(chefEntity.position.value, targetPosition) > Mathf.Epsilon;
}
