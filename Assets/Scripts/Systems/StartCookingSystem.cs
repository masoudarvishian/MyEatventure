using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

internal class StartCookingSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _customerGroup;
    private CompositeDisposable _compositeDisposable = new();

    public StartCookingSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        _customerGroup = _contexts.game.GetGroup(GameMatcher.Customer);
    }

    ~StartCookingSystem()
    {
        _compositeDisposable.Dispose();
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in entities)
        {
            if (HasReachedToTargetPosition(chefEntity, GetRestaurantTargetPosition().GetFirstKitchenSpot().position))
            {
                var busyKitchenEntity = _contexts.game.CreateEntity();
                busyKitchenEntity.isBuysKitchen = true;

                var cooldownDuration = 2f;
                Observable.Timer(TimeSpan.FromSeconds(cooldownDuration)).Subscribe(_ => {
                    if (!chefEntity.hasCustomerIndex || _customerGroup.GetEntities().Length == 0)
                        return;
                    var chefCustomerEntity = _customerGroup.GetEntities().First(x => x.creationIndex == chefEntity.customerIndex.value);
                    chefEntity.ReplaceTargetPosition(chefCustomerEntity.targetDeskPosition.value);
                    busyKitchenEntity.Destroy();
                }).AddTo(_compositeDisposable);
            }
        }
    }

    protected override bool Filter(GameEntity entity) => !entity.hasTargetPosition && entity.isChef;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.TargetPosition).AnyOf(GameMatcher.Chef).Removed());

    private bool HasReachedToTargetPosition(GameEntity entity, Vector3 targetPosition) =>
        Vector3.Distance(entity.position.value, targetPosition) <= Mathf.Epsilon;

    private RestaurantTargetPositions GetRestaurantTargetPosition() =>
        _contexts.game.GetGroup(GameMatcher.Restaurant).GetEntities().First().visual.gameObject.GetComponent<RestaurantTargetPositions>();
}
