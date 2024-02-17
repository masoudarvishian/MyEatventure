using Entitas;
using UnityEngine;
using UniRx;

internal class MovingChefSystem : IExecuteSystem, IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _chefWithTargetPositionGroup;
    private readonly IGroup<GameEntity> _chefGroup;
    private readonly CompositeDisposable _compositeDisposable = new();
    private float _speed = 4.0f;

    public MovingChefSystem(Contexts contexts)
    {
        _contexts = contexts;
        _chefWithTargetPositionGroup = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef).AnyOf(GameMatcher.TargetPosition));
        _chefGroup = _contexts.game.GetGroup(GameMatcher.Chef);
    }

    ~MovingChefSystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
    }

    public void Execute()
    {
        MoveAllChefs();
    }

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => FreeTheChefs()).AddTo(_compositeDisposable);
    }

    private void FreeTheChefs()
    {
        foreach (var chefEntity in _chefGroup.GetEntities())
        {
            if (chefEntity.hasTargetPosition)
                chefEntity.RemoveTargetPosition();
            if (chefEntity.hasCustomerIndex)
                chefEntity.RemoveCustomerIndex();
        }
    }

    private void MoveAllChefs()
    {
        foreach (var chefEntity in _chefWithTargetPositionGroup.GetEntities())
        {
            MoveEntity(chefEntity, chefEntity.targetPosition.value);
            if (HasReachedToTargetPosition(chefEntity.position.value, chefEntity.targetPosition.value))
                chefEntity.RemoveTargetPosition();
        }
    }

    private void MoveEntity(GameEntity entity, Vector3 targetPosition)
    {
        var entityTransform = entity.visual.gameObject.transform;
        entityTransform.position = Vector3.MoveTowards(entityTransform.position, targetPosition, GetStep());
        entity.position.value = entityTransform.position;
    }

    private float GetStep() => _speed * Time.deltaTime;

    private static bool HasReachedToTargetPosition(Vector3 entityPosition, Vector3 targetPosition) =>
        Vector3.Distance(entityPosition, targetPosition) <= Mathf.Epsilon;
}
