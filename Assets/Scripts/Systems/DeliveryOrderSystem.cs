using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class DeliveryOrderSystem : ReactiveSystem<GameEntity>
{
    private IGroup<GameEntity> _waitingCustomersGroup;

    public DeliveryOrderSystem(Contexts contexts) : base(contexts.game)
    {
        _waitingCustomersGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Customer).AnyOf(GameMatcher.PreparingOrder));
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in entities)
            ReduceQuantityIfHasDeliveredOrder(chefEntity);
    }

    protected override bool Filter(GameEntity entity) => !entity.hasTargetPosition && entity.isChef;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.TargetPosition).AnyOf(GameMatcher.Chef).Removed());

    private void ReduceQuantityIfHasDeliveredOrder(GameEntity chefEntity)
    {
        var chefCustomers = _waitingCustomersGroup.GetEntities().Where(x => x.creationIndex == chefEntity.customerIndex.value);
        foreach (var customerEntity in chefCustomers)
        {
            if (HasReachedToTargetPosition(chefEntity, customerEntity.targetDeskPosition.value))
            {
                RecudeQuantity(customerEntity);
                HandleDeliveryComponents(chefEntity, customerEntity);
            }
        }
    }

    private static bool HasReachedToTargetPosition(GameEntity chefEntity, Vector3 targetPosition) =>
        Vector3.Distance(chefEntity.position.value, targetPosition) <= Mathf.Epsilon;

    private static void RecudeQuantity(GameEntity customerEntity)
    {
        if (customerEntity.quantity.value > 0)
            customerEntity.quantity.value--;
    }

    private static void HandleDeliveryComponents(GameEntity chefEntity, GameEntity customerEntity)
    {
        if (customerEntity.quantity.value != 0)
            return;

        customerEntity.ReplaceDelivered(true);
        chefEntity.RemoveCustomerIndex();
    }
}
