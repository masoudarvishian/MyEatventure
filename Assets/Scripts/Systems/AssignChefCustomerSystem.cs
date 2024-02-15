using Entitas;
using System.Collections.Generic;
using System.Linq;

public sealed class AssignChefCustomerSystem : IExecuteSystem
{
    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _freeChefGroup;
    private readonly Queue<GameEntity> _waitingCustomersQueue = new();

    public AssignChefCustomerSystem(Contexts contexts)
    {
        _contexts = contexts;
        _freeChefGroup = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef).NoneOf(GameMatcher.CustomerIndex));
        _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Customer).AnyOf(GameMatcher.Waiting)).OnEntityAdded += OnWaitingCustomerAdded;
    }

    public void Execute()
    {
        for (int i = 0; i < _waitingCustomersQueue.Count; i++)
        {
            var waitingCustomerEntity = _waitingCustomersQueue.ElementAt(i);
            foreach (var freeChefEntity in _freeChefGroup.GetEntities())
            {
                if (_waitingCustomersQueue.Count == 0)
                    break;

                freeChefEntity.AddCustomerIndex(waitingCustomerEntity.creationIndex);
                freeChefEntity.AddTargetPosition(waitingCustomerEntity.targetDeskPosition.value);
                _waitingCustomersQueue.Dequeue();
            }
        }
    }

    private void OnWaitingCustomerAdded(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
    {
        _waitingCustomersQueue.Enqueue(entity);
    }
}