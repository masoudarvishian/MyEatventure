using Entitas;
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
        foreach (var customerEntity in GetPendingCustomers())
        {
            var freeKitchens = GetFreeKitchens();
            if (ChefIsDeliveringFirstOrderToCustomer(chefEntity, customerEntity))
            {
                HandleDeliveringFirstOrder(chefEntity, freeKitchens);
                continue;
            }
            else if (ShouldTakeTheOrder(chefEntity, customerEntity))
                HandleTakingTheOrder(chefEntity, customerEntity, freeKitchens);
        }
    }

    private void HandleTakingTheOrder(GameEntity chefEntity, GameEntity customerEntity, IEnumerable<GameEntity> freeKitchens)
    {
        if (freeKitchens.Count() <= 0)
            AddChefToQueue(chefEntity);
        else
        {
            var targetKitchenPos = freeKitchens.First().visual.gameObject.transform.position;
            _kitchenGroup.GetEntities().First().isBuysKitchen = true;
            EntityCooldown(chefEntity, COOLDOWN_TAKING_ORDER)
                .Subscribe(_ =>
                {
                    GoToKitchen(chefEntity, targetKitchenPos);
                    UpdateTakingOrderComponents(customerEntity);
                }).AddTo(_compositeDisposable);
        }
    }

    private void HandleDeliveringFirstOrder(GameEntity chefEntity, IEnumerable<GameEntity> freeKitchens)
    {
        if (freeKitchens.Count() <= 0)
            AddChefToQueue(chefEntity);
        else
        {
            var targetKitchenPos = freeKitchens.First().visual.gameObject.transform.position;
            _kitchenGroup.GetEntities().First().isBuysKitchen = true;
            EntityCooldown(chefEntity, COOLDOWN_FIRST_DELIVERY)
                .Subscribe(_ =>
                {
                    GoToKitchen(chefEntity, targetKitchenPos);
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
        chefEntity.AddCooldown(cooldownDuration);
        return Observable.Timer(TimeSpan.FromSeconds(cooldownDuration))
            .DoOnCompleted(() => 
            {
                chefEntity.RemoveCooldown();
            });
    }

    private static bool ShouldTakeTheOrder(GameEntity chefEntity, GameEntity customerEntity) =>
        HasReachedToTargetPosition(chefEntity, customerEntity.targetDeskPosition.value);

    private void GoToKitchen(GameEntity chefEntity, Vector3 kitchenPosition)
    {
        chefEntity.ReplaceTargetPosition(kitchenPosition);
    }

    private IEnumerable<GameEntity> GetFreeKitchens() =>
        _kitchenGroup.GetEntities().Where(x => !x.isBuysKitchen);

    private static void UpdateTakingOrderComponents(GameEntity customerEntity)
    {
        customerEntity.isPreparingOrder = true;
        customerEntity.isWaiting = false;
    }
}
