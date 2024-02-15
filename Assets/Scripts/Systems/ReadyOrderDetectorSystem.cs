using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ReadyOrderDetectorSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;
    private readonly IGroup<GameEntity> _waitingCustomerGroup;
    private readonly IGroup<GameEntity> _chefGroup;

    public ReadyOrderDetectorSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions) : base(contexts.game)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
        _waitingCustomerGroup = _contexts.game.GetGroup(GameMatcher.Customer);
        _chefGroup = _contexts.game.GetGroup(GameMatcher.Chef);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in _chefGroup.GetEntities())
        {
            if (HasReachedToTargetPosition(chefEntity, _restaurantTargetPositions.GetFirstKitchenSpot().position))
            {
                var chefCustomerEntity = _waitingCustomerGroup.GetEntities().First(x => x.creationIndex == chefEntity.customerIndex.value);
                chefEntity.AddTargetPosition(chefCustomerEntity.targetDeskPosition.value);
            }
        }
    }

    protected override bool Filter(GameEntity entity) => entity.isChef;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.Cooldown).Removed());

    private bool HasReachedToTargetPosition(GameEntity chefEntity, Vector3 targetPosition) =>
        Vector3.Distance(chefEntity.position.value, targetPosition) <= Mathf.Epsilon;
}
