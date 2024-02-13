using Entitas;
using System.Collections.Generic;
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
        _waitingCustomerGroup = _contexts.game.GetGroup(GameMatcher.WaitingCustomer);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in _contexts.game.GetGroup(GameMatcher.Chef).GetEntities())
        {
            if (Vector3.Distance(chefEntity.position.value, _restaurantTargetPositions.GetFirstKitchenSpot().position) <= Mathf.Epsilon)
            {
                var waitingCustomerEntity = _waitingCustomerGroup.GetEntities()[0];
                var e = _contexts.game.CreateEntity();
                e.AddTargetPosition(waitingCustomerEntity.waitingCustomer.position);
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
