using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class TakingOrderDetectorSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private IGroup<GameEntity> _waitingCustomersGroup;

    public TakingOrderDetectorSystem(Contexts contexts) : base(contexts.game)
    {
        _waitingCustomersGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.WaitingCustomer));
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            foreach (var waitingCustomerEntity in _waitingCustomersGroup.GetEntities().Where(x => x.quantity.value > 0))
            {
                if (Vector3.Distance(entity.position.value, waitingCustomerEntity.waitingCustomer.position) <= Mathf.Epsilon &&
                    waitingCustomerEntity.isPreparingOrder)
                {
                    var cooldownEntity = _contexts.game.CreateEntity();
                    cooldownEntity.AddCooldown(0.1f);
                    continue;
                }

                if (Vector3.Distance(entity.position.value, waitingCustomerEntity.waitingCustomer.position) <= Mathf.Epsilon)
                {
                    var cooldownEntity = _contexts.game.CreateEntity();
                    cooldownEntity.AddCooldown(2f);
                    waitingCustomerEntity.isPreparingOrder = true;
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
