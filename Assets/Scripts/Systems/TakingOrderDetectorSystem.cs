using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using System;

public sealed class TakingOrderDetectorSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private IGroup<GameEntity> _waitingCustomersGroup;
    private CompositeDisposable _compositeDisposable = new();

    public TakingOrderDetectorSystem(Contexts contexts) : base(contexts.game)
    {
        _waitingCustomersGroup = contexts.game.GetGroup(GameMatcher.Customer);
        _contexts = contexts;
    }

    ~TakingOrderDetectorSystem()
    {
        _compositeDisposable.Dispose();
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in entities)
            TryToAddCooldownIfChefHasReachedToCustomer(chefEntity);
    }

    private void TryToAddCooldownIfChefHasReachedToCustomer(GameEntity chefEntity)
    {
        foreach (var customerEntity in _waitingCustomersGroup.GetEntities().Where(x => x.quantity.value > 0))
        {
            if (HasReachedToTargetPosition(chefEntity, customerEntity.targetDeskPosition.value) && customerEntity.isPreparingOrder)
            {
                var cooldownDuration = 0.1f;
                chefEntity.AddCooldown(cooldownDuration);
                Observable.Timer(TimeSpan.FromSeconds(cooldownDuration)).Subscribe(_ => {
                    chefEntity.RemoveCooldown();
                }).AddTo(_compositeDisposable);

                continue;
            }

            if (HasReachedToTargetPosition(chefEntity, customerEntity.targetDeskPosition.value))
            {
                var cooldownDuration = 1f;
                chefEntity.AddCooldown(cooldownDuration);
                Observable.Timer(TimeSpan.FromSeconds(cooldownDuration)).Subscribe(_ => {
                    chefEntity.RemoveCooldown();
                }).AddTo(_compositeDisposable);

                customerEntity.isPreparingOrder = true;
                customerEntity.isWaiting = false;
            }
        }
    }

    private static bool HasReachedToTargetPosition(GameEntity entity, Vector3 targetPosition) =>
        Vector3.Distance(entity.position.value, targetPosition) <= Mathf.Epsilon;

    protected override bool Filter(GameEntity entity) => !entity.hasTargetPosition && entity.isChef;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.TargetPosition).AnyOf(GameMatcher.Chef).Removed());
}
