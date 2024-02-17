using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using System;

public sealed class TakingOrderDetectorSystem : ReactiveSystem<GameEntity>, IInitializeSystem
{
    private readonly Contexts _contexts;
    private IGroup<GameEntity> _customersGroup;
    private IGroup<GameEntity> _restaurantGroup;
    private CompositeDisposable _compositeDisposable = new();
    private RestaurantTargetPositions _restaurantTargetPositions;

    private const float COOLDOWN_TAKING_ORDER = 1f;
    private const float COOLDOWN_FIRST_DELIVERY = 0.1f;

    public TakingOrderDetectorSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        _customersGroup = _contexts.game.GetGroup(GameMatcher.Customer);
        _restaurantGroup = _contexts.game.GetGroup(GameMatcher.Restaurant);
    }

    ~TakingOrderDetectorSystem()
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
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.TargetPosition).AnyOf(GameMatcher.Chef).Removed());

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => ResetRestaurantTargetPosition()).AddTo(_compositeDisposable);
    }

    private void CheckToTakeOrderFromPendingCustomers(GameEntity chefEntity)
    {
        ResetRestaurantTargetPosition();
        foreach (var customerEntity in GetPendingCustomers())
        {
            if (ChefIsDeliveringFirstOrderToCustomer(chefEntity, customerEntity))
            {
                EntityCooldown(chefEntity, COOLDOWN_FIRST_DELIVERY);
                continue;
            }

            if (ShouldTakeTheOrder(chefEntity, customerEntity))
            {
                EntityCooldown(chefEntity, COOLDOWN_TAKING_ORDER);
                UpdateTakingOrderComponents(customerEntity);
            }
        }
    }

    private void ResetRestaurantTargetPosition()
    {
        _restaurantTargetPositions = GetRestaurantTargetPosition();
    }

    private RestaurantTargetPositions GetRestaurantTargetPosition() =>
       _restaurantGroup.GetEntities().First().visual.gameObject.GetComponent<RestaurantTargetPositions>();

    private IEnumerable<GameEntity> GetPendingCustomers() =>
        _customersGroup.GetEntities().Where(x => x.quantity.value > 0);

    private static bool ChefIsDeliveringFirstOrderToCustomer(GameEntity chefEntity, GameEntity customerEntity) =>
        HasReachedToTargetPosition(chefEntity, customerEntity.targetDeskPosition.value) && customerEntity.isPreparingOrder;

    private static bool HasReachedToTargetPosition(GameEntity entity, Vector3 targetPosition) =>
        Vector3.Distance(entity.position.value, targetPosition) <= Mathf.Epsilon;


    private void EntityCooldown(GameEntity chefEntity, float cooldownDuration)
    {
        chefEntity.AddCooldown(cooldownDuration);
        Observable.Timer(TimeSpan.FromSeconds(cooldownDuration)).Subscribe(_ =>
        {
            chefEntity.RemoveCooldown();
            GoToKitchen(chefEntity);
        }).AddTo(_compositeDisposable);
    }

    private static bool ShouldTakeTheOrder(GameEntity chefEntity, GameEntity customerEntity) =>
        HasReachedToTargetPosition(chefEntity, customerEntity.targetDeskPosition.value);

    private void GoToKitchen(GameEntity chefEntity)
    {
        chefEntity.ReplaceTargetPosition(GetFirstKitchenSpotPosition());
    }

    private Vector3 GetFirstKitchenSpotPosition() =>
        _restaurantTargetPositions.GetFirstKitchenSpot().position;

    private static void UpdateTakingOrderComponents(GameEntity customerEntity)
    {
        customerEntity.isPreparingOrder = true;
        customerEntity.isWaiting = false;
    }
}
