using Entitas;
using UnityEngine;

public sealed class TargetPositionEmitterSystem : IExecuteSystem
{
    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _waitingCustomerGroup;

    public TargetPositionEmitterSystem(Contexts contexts)
    {
        _contexts = contexts;
        _waitingCustomerGroup = _contexts.game.GetGroup(GameMatcher.WaitingCustomer);
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var waitingCustomerEntity in _waitingCustomerGroup.GetEntities())
            {
                var e = _contexts.game.CreateEntity();
                e.AddTargetPosition(waitingCustomerEntity.waitingCustomer.position);
                return;
            }
        }
    }
}
