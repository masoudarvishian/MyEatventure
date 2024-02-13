using Entitas;
using System.Collections.Generic;
using UnityEngine;

public sealed class DeliveryOrderSystem : ReactiveSystem<GameEntity>
{
    private IGroup<GameEntity> _waitingCustomersGroup;

    public DeliveryOrderSystem(Contexts contexts) : base(contexts.game)
    {
        _waitingCustomersGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.WaitingCustomer).AnyOf(GameMatcher.PreparingOrder));
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            foreach (var waitingCustomerEntity in _waitingCustomersGroup.GetEntities())
            {
                if (Vector3.Distance(entity.position.value, waitingCustomerEntity.waitingCustomer.position) <= Mathf.Epsilon)
                {
                    if (waitingCustomerEntity.quantity.value > 0)
                        waitingCustomerEntity.quantity.value--;

                    if (waitingCustomerEntity.quantity.value == 0)
                        waitingCustomerEntity.ReplaceDelivered(true);
                }
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
