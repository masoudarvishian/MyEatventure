using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public sealed class DeliveryOrderSystem : ReactiveSystem<GameEntity>
{
    public static IObservable<Unit> OnOrderIsDelivered => _onOrderIsDeliverdSubject;

    private IGroup<GameEntity> _customersGroup;
    private IGroup<GameEntity> _frontDeskGroup;
    private readonly Contexts _contexts;
    private static Subject<Unit> _onOrderIsDeliverdSubject = new Subject<Unit>();
    
    public DeliveryOrderSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        _customersGroup = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Customer).AnyOf(GameMatcher.PreparingOrder));
        _frontDeskGroup = _contexts.game.GetGroup(GameMatcher.FrontDeskSpot);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in entities)
            CheckOrderDelivery(chefEntity);
    }

    protected override bool Filter(GameEntity entity) => entity.isChef && entity.hasCustomerIndex;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.TargetPosition).AnyOf(GameMatcher.Chef).Removed());

    private void CheckOrderDelivery(GameEntity chefEntity)
    {
        foreach (var customerEntity in GetCustomerEntitiesOf(chefEntity))
        {
            if (HasReachedToTargetPosition(chefEntity, customerEntity.targetDeskPosition.value))
            {
                HandleDelivery(chefEntity, customerEntity);
                break;
            }
        }
    }

    private IEnumerable<GameEntity> GetCustomerEntitiesOf(GameEntity chefEntity) =>
        _customersGroup.GetEntities().Where(x => x.creationIndex == chefEntity.customerIndex.value);

    private static bool HasReachedToTargetPosition(GameEntity chefEntity, Vector3 targetPosition) =>
        Vector3.Distance(chefEntity.position.value, targetPosition) <= Mathf.Epsilon;

    private void HandleDelivery(GameEntity chefEntity, GameEntity customerEntity)
    {
        _onOrderIsDeliverdSubject?.OnNext(Unit.Default);
        RecudeQuantity(customerEntity);
        HandleDeliveryComponents(chefEntity, customerEntity);
    }

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
        GetCustomerFronDeskEntity(customerEntity).isOccupied = false;
        customerEntity.ReplaceDelivered(true);
        chefEntity.RemoveCustomerIndex();
    }

    private GameEntity GetCustomerFronDeskEntity(GameEntity customerEntity) =>
        _frontDeskGroup.GetEntities().First(x => x.index.value == customerEntity.targetDeskIndex.value);

    private static void UpdateQuantityUI(GameEntity customerEntity)
    {
        customerEntity.visual.gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = customerEntity.quantity.value.ToString();
    }
}
