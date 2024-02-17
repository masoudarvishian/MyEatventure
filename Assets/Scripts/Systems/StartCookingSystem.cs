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
    private readonly IGroup<GameEntity> _restaurantGroup;
    private readonly IGroup<GameEntity> _kitchenGroup;
    private CompositeDisposable _compositeDisposable = new();

    private const float COOLDOWN_DURATION = 2f;

    public StartCookingSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
        _customerGroup = _contexts.game.GetGroup(GameMatcher.Customer);
        _restaurantGroup = _contexts.game.GetGroup(GameMatcher.Restaurant);
        _kitchenGroup = _contexts.game.GetGroup(GameMatcher.Kitchen);
    }

    ~StartCookingSystem()
    {
        _compositeDisposable.Dispose();
    }

    protected override void Execute(List<GameEntity> chefEntities)
    {
        CheckIfChefsShouldStartCooking(chefEntities);
    }

    protected override bool Filter(GameEntity entity) => !entity.hasTargetPosition && entity.isChef;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.TargetPosition).AnyOf(GameMatcher.Chef).Removed());

    private void CheckIfChefsShouldStartCooking(List<GameEntity> chefEntities)
    {
        foreach (var chefEntity in chefEntities)
        {
            if (HasReachedToTargetPosition(chefEntity, GetFirstKitchenPosition()))
                StartCooking(chefEntity);
        }
    }

    private bool HasReachedToTargetPosition(GameEntity entity, Vector3 targetPosition) =>
        Vector3.Distance(entity.position.value, targetPosition) <= Mathf.Epsilon;

    private Vector3 GetFirstKitchenPosition() =>
        _kitchenGroup.GetEntities().First().visual.gameObject.transform.position;

    private void StartCooking(GameEntity chefEntity)
    {
        chefEntity.AddCooldown(COOLDOWN_DURATION);
        Observable.Timer(TimeSpan.FromSeconds(COOLDOWN_DURATION))
            .Subscribe(_ =>
            {
                chefEntity.RemoveCooldown();
                if (NoCustomerExistsFor(chefEntity)) return;
                GoBackToCustomer(chefEntity);
            })
            .AddTo(_compositeDisposable);
    }

    private bool NoCustomerExistsFor(GameEntity chefEntity) =>
        !chefEntity.hasCustomerIndex || _customerGroup.GetEntities().Length == 0;

    private void GoBackToCustomer(GameEntity chefEntity)
    {
        chefEntity.ReplaceTargetPosition(GetChefCustomerTargetDeskPosition(chefEntity));
    }

    private Vector3 GetChefCustomerTargetDeskPosition(GameEntity chefEntity) =>
        GetChefCustomerEntity(chefEntity).targetDeskPosition.value;

    private GameEntity GetChefCustomerEntity(GameEntity chefEntity) =>
       _customerGroup.GetEntities().First(x => x.creationIndex == chefEntity.customerIndex.value);

    private RestaurantTargetPositions GetRestaurantTargetPosition() =>
        _restaurantGroup.GetEntities().First().visual.gameObject.GetComponent<RestaurantTargetPositions>();
}
