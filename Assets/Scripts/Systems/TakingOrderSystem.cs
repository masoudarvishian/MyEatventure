﻿using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using System;

public sealed class TakingOrderSystem : ReactiveSystem<GameEntity>, IInitializeSystem
{
    private readonly Contexts _contexts;
    private IGroup<GameEntity> _customersGroup;
    private IGroup<GameEntity> _kitchenGroup;
    private CompositeDisposable _compositeDisposable = new();
    private IDisposable _cooldownDisposable;
    private readonly Queue<GameEntity> _chefEntityQueue = new Queue<GameEntity>();

    private const float COOLDOWN_TAKING_ORDER = 1f;
    private const float COOLDOWN_FIRST_DELIVERY = 0.1f;

    public TakingOrderSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        _customersGroup = _contexts.game.GetGroup(GameMatcher.Customer);
        _kitchenGroup = _contexts.game.GetGroup(GameMatcher.Kitchen);
    }

    ~TakingOrderSystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var chefEntity in entities)
            CheckToTakeOrderFromPendingCustomers(chefEntity);
    }

    protected override bool Filter(GameEntity entity) => !entity.hasTargetPosition && entity.isChef;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.Chef, GameMatcher.TargetPosition).Removed());

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => _cooldownDisposable.Dispose()).AddTo(_compositeDisposable);
        StartCookingSystem.OnKitchenGetsFree
            .Subscribe(_ =>
            {
                if (_chefEntityQueue.Count > 0)
                    CheckToTakeOrderFromPendingCustomers(_chefEntityQueue.Dequeue());
            })
            .AddTo(_compositeDisposable);
    }

    private void CheckToTakeOrderFromPendingCustomers(GameEntity chefEntity)
    {
        var freeKitchens = GetFreeKitchens();
        foreach (var customerEntity in GetPendingCustomers())
        {
            if (ChefIsDeliveringFirstOrderToCustomer(chefEntity, customerEntity))
            {
                HandleDeliveringFirstOrder(chefEntity, freeKitchens);
                continue;
            }
            else if (ShouldTakeTheOrder(chefEntity, customerEntity))
                HandleTakingTheOrder(chefEntity, customerEntity, freeKitchens);
        }
    }

    private void HandleDeliveringFirstOrder(GameEntity chefEntity, IEnumerable<GameEntity> freeKitchens)
    {
        if (freeKitchens.Count() == 0)
            AddChefToQueue(chefEntity);
        else
        {
            var freeKitchen = freeKitchens.First();
            var targetKitchenPos = freeKitchen.visual.gameObject.transform.position;
            var kitchenIndex = freeKitchen.index.value;
            freeKitchen.isBuysKitchen = true;
            _cooldownDisposable = EntityCooldown(chefEntity, COOLDOWN_FIRST_DELIVERY)
                .Subscribe(_ =>
                {
                    GoToKitchen(chefEntity, targetKitchenPos, kitchenIndex);
                }).AddTo(_compositeDisposable);
        }
    }

    private void HandleTakingTheOrder(GameEntity chefEntity, GameEntity customerEntity, IEnumerable<GameEntity> freeKitchens)
    {
        if (freeKitchens.Count() == 0)
            AddChefToQueue(chefEntity);
        else
        {
            var freeKitchen = freeKitchens.First();
            var targetKitchenPos = freeKitchen.visual.gameObject.transform.position;
            var kitchenIndex = freeKitchen.index.value;
            freeKitchen.isBuysKitchen = true;
            _cooldownDisposable = EntityCooldown(chefEntity, COOLDOWN_TAKING_ORDER)
                .Subscribe(_ =>
                {
                    GoToKitchen(chefEntity, targetKitchenPos, kitchenIndex);
                    UpdateTakingOrderComponents(customerEntity);
                }).AddTo(_compositeDisposable);
        }
    }

    private void AddChefToQueue(GameEntity chefEntity)
    {
        if (_chefEntityQueue.Contains(chefEntity))
            return;
        _chefEntityQueue.Enqueue(chefEntity);
    }

    private IEnumerable<GameEntity> GetPendingCustomers() =>
        _customersGroup.GetEntities().Where(x => x.quantity.value > 0);

    private static bool ChefIsDeliveringFirstOrderToCustomer(GameEntity chefEntity, GameEntity customerEntity) =>
        HasReachedToTargetPosition(chefEntity, customerEntity.targetDeskPosition.value) && customerEntity.isPreparingOrder;

    private static bool HasReachedToTargetPosition(GameEntity entity, Vector3 targetPosition) =>
        Vector3.Distance(entity.position.value, targetPosition) <= Mathf.Epsilon;


    private IObservable<long> EntityCooldown(GameEntity chefEntity, float cooldownDuration)
    {
        chefEntity.ReplaceCooldown(cooldownDuration);
        return Observable.Timer(TimeSpan.FromSeconds(cooldownDuration))
            .DoOnCompleted(() => 
            {
                if (chefEntity.hasCooldown)
                    chefEntity.RemoveCooldown();
            });
    }

    private static bool ShouldTakeTheOrder(GameEntity chefEntity, GameEntity customerEntity) =>
        HasReachedToTargetPosition(chefEntity, customerEntity.targetDeskPosition.value);

    private void GoToKitchen(GameEntity chefEntity, Vector3 kitchenPosition, int kitchenIndex)
    {
        chefEntity.ReplaceTargetPosition(kitchenPosition);
        chefEntity.ReplaceKitchenIndex(kitchenIndex);
    }

    private IEnumerable<GameEntity> GetFreeKitchens() =>
        _kitchenGroup.GetEntities().Where(x => !x.isBuysKitchen);

    private static void UpdateTakingOrderComponents(GameEntity customerEntity)
    {
        if (!customerEntity.isEnabled)
            return;

        customerEntity.isPreparingOrder = true;
        customerEntity.isWaiting = false;
    }
}
