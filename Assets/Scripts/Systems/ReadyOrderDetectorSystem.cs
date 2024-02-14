using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ReadyOrderDetectorSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;
    private readonly IGroup<GameEntity> _waitingCustomerGroup;

    public ReadyOrderDetectorSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions) : base(contexts.game)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
        _waitingCustomerGroup = _contexts.game.GetGroup(GameMatcher.Customer);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in _contexts.game.GetGroup(GameMatcher.Chef).GetEntities())
        {
            if (HasReachedToTargetPosition(chefEntity, _restaurantTargetPositions.GetFirstKitchenSpot().position))
            {
                var chefCustomerEntity = _waitingCustomerGroup.GetEntities().First(x => x.creationIndex == chefEntity.customerIndex.value);
                AddTargetPositionEntity(chefCustomerEntity.position.value);
            }
        }
    }

    protected override bool Filter(GameEntity entity) => !entity.isEnabled;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.Cooldown).Removed());

    private bool HasReachedToTargetPosition(GameEntity chefEntity, Vector3 targetPosition) =>
        Vector3.Distance(chefEntity.position.value, targetPosition) <= Mathf.Epsilon;

    private void AddTargetPositionEntity(Vector3 value)
    {
        var e = _contexts.game.CreateEntity();
        e.AddTargetPosition(value);
    }
}
