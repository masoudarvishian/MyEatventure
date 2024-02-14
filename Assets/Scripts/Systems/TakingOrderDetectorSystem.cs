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
        _waitingCustomersGroup = contexts.game.GetGroup(GameMatcher.Customer);
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in entities)
            TryToAddCooldownIfChefHasReachedToCustomer(chefEntity);
    }

    private void TryToAddCooldownIfChefHasReachedToCustomer(GameEntity chefEntity)
    {
        foreach (var waitingCustomerEntity in _waitingCustomersGroup.GetEntities().Where(x => x.quantity.value > 0))
        {
            if (HasReachedToTargetPosition(chefEntity, waitingCustomerEntity.position.value) && waitingCustomerEntity.isPreparingOrder)
            {
                AddCooldownEntity(0.1f);
                continue;
            }

            if (HasReachedToTargetPosition(chefEntity, waitingCustomerEntity.position.value))
            {
                AddCooldownEntity(2f);
                waitingCustomerEntity.isPreparingOrder = true;
            }
        }
    }

    private void AddCooldownEntity(float value)
    {
        var cooldownEntity = _contexts.game.CreateEntity();
        cooldownEntity.AddCooldown(value);
    }

    private static bool HasReachedToTargetPosition(GameEntity entity, Vector3 targetPosition) =>
        Vector3.Distance(entity.position.value, targetPosition) <= Mathf.Epsilon;

    protected override bool Filter(GameEntity entity) => !entity.isMover && entity.isChef;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.Mover).AnyOf(GameMatcher.Chef).Removed());
}
