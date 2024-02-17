using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

internal class StartCookingSystem : ReactiveSystem<GameEntity>
{
    public static IObservable<Unit> OnKitchenGetsFree => _onKitchenGetsFree;

    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _customerGroup;
    private readonly IGroup<GameEntity> _restaurantGroup;
    private readonly IGroup<GameEntity> _kitchenGroup;
    private CompositeDisposable _compositeDisposable = new();
    private static ISubject<Unit> _onKitchenGetsFree = new Subject<Unit>();

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

    protected override bool Filter(GameEntity entity) => !entity.hasTargetPosition && entity.isChef && entity.hasKitchenIndex;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.TargetPosition).AnyOf(GameMatcher.Chef).Removed());

    private void CheckIfChefsShouldStartCooking(List<GameEntity> chefEntities)
    {
        foreach (var chefEntity in chefEntities)
        {
            var kitchen = GetChefKitchen(chefEntity);
            if (HasReachedToTargetPosition(chefEntity, GetKitchenPosition(kitchen)))
                StartCooking(chefEntity, kitchen);
        }
    }

    private bool HasReachedToTargetPosition(GameEntity entity, Vector3 targetPosition) =>
        Vector3.Distance(entity.position.value, targetPosition) <= Mathf.Epsilon;

    private Vector3 GetKitchenPosition(GameEntity freeKitchen) =>
        freeKitchen.visual.gameObject.transform.position;

    private void StartCooking(GameEntity chefEntity, GameEntity kitchenEntity)
    {
        chefEntity.AddCooldown(COOLDOWN_DURATION);
        Observable.Timer(TimeSpan.FromSeconds(COOLDOWN_DURATION))
            .Subscribe(_ =>
            {
                chefEntity.RemoveKitchenIndex();
                chefEntity.RemoveCooldown();
                kitchenEntity.isBuysKitchen = false;
                _onKitchenGetsFree?.OnNext(Unit.Default);
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

    private GameEntity GetChefKitchen(GameEntity chefEntity) =>
        _kitchenGroup.GetEntities().Where(x => x.index.value == chefEntity.kitchenIndex.value).First();
}
