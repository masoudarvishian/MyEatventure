using Entitas;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public sealed class AssignChefCustomerSystem : IExecuteSystem, IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _freeChefGroup;
    private readonly Queue<GameEntity> _waitingCustomersQueue = new();
    private readonly CompositeDisposable _compositeDisposable = new();

    public AssignChefCustomerSystem(Contexts contexts)
    {
        _contexts = contexts;
        _freeChefGroup = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef).NoneOf(GameMatcher.CustomerIndex));
        _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Customer).AnyOf(GameMatcher.Waiting)).OnEntityAdded += OnWaitingCustomerAdded;
    }

    ~AssignChefCustomerSystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickRestaurantUpgrade
                    .Subscribe(_ =>
                    {
                        _waitingCustomersQueue.Clear();
                    }).AddTo(_compositeDisposable);
    }

    public void Execute()
    {
        for (int i = 0; i < _waitingCustomersQueue.Count; i++)
            AssignCustomerToFreeChef(i);
    }

    private void AssignCustomerToFreeChef(int index)
    {
        var waitingCustomerEntity = _waitingCustomersQueue.ElementAt(index);
        var freeChefEntity = GetClosestChef(waitingCustomerEntity.position.value, _freeChefGroup.GetEntities());

        if (IsNotEligibleToAssign(freeChefEntity))
            return;

        freeChefEntity.AddCustomerIndex(waitingCustomerEntity.creationIndex);
        freeChefEntity.ReplaceTargetPosition(waitingCustomerEntity.targetDeskPosition.value);
        _waitingCustomersQueue.Dequeue();
    }

    private bool IsNotEligibleToAssign(GameEntity freeChefEntity) =>
        _waitingCustomersQueue.Count == 0 || freeChefEntity == null;

    private void OnWaitingCustomerAdded(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
    {
        _waitingCustomersQueue.Enqueue(entity);
    }

    private GameEntity GetClosestChef(Vector3 customerPosition, GameEntity[] chefEntities)
    {
        GameEntity closestChef = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = customerPosition;
        foreach (GameEntity e in chefEntities)
        {
            float dist = Vector3.Distance(e.position.value, currentPos);
            if (dist < minDist)
            {
                closestChef = e;
                minDist = dist;
            }
        }
        return closestChef;
    }
}