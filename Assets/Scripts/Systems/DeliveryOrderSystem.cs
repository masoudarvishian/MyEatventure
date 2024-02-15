using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class DeliveryOrderSystem : ReactiveSystem<GameEntity>
{
    private IGroup<GameEntity> _customersGroup;
    private readonly Contexts _contexts;

    public DeliveryOrderSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        _customersGroup = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Customer).AnyOf(GameMatcher.PreparingOrder));
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
        var chefCustomers = _customersGroup.GetEntities().Where(x => x.creationIndex == chefEntity.customerIndex.value);
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
        UpdateQuantityUI(customerEntity);
    }

    private void HandleDeliveryComponents(GameEntity chefEntity, GameEntity customerEntity)
    {
        if (customerEntity.quantity.value != 0)
            return;

        var frontDeskEntity = _contexts.game.GetGroup(GameMatcher.FrontDeskSpot).GetEntities().First(x => x.index.value == customerEntity.targetDeskIndex.value);
        frontDeskEntity.isOccupied = false;
        customerEntity.ReplaceDelivered(true);
        chefEntity.RemoveCustomerIndex();
    }

    private static void UpdateQuantityUI(GameEntity customerEntity)
    {
        customerEntity.visual.gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = customerEntity.quantity.value.ToString();
    }
}
