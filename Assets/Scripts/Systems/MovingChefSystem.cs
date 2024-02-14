using Entitas;
using System.Linq;
using UnityEngine;

internal class MovingChefSystem : IExecuteSystem
{
    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _chefGroup;
    private float _speed = 3.0f;

    public MovingChefSystem(Contexts contexts)
    {
        _contexts = contexts;
        _chefGroup = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef));
    }

    public void Execute()
    {
        foreach (var entity in _chefGroup.GetEntities().Where(x => x.hasTargetPosition))
        {
            MoveEntity(entity);
            if (HasReachedToTargetPosition(entity.position.value, entity.targetPosition.value))
                entity.RemoveTargetPosition();
        }
    }

    private void MoveEntity(GameEntity entity)
    {
        var entityTransform = entity.visual.gameObject.transform;
        entityTransform.position = Vector3.MoveTowards(entityTransform.position, entity.targetPosition.value, GetStep());
        entity.position.value = entityTransform.position;
    }

    private float GetStep() => _speed * Time.deltaTime;

    private static bool HasReachedToTargetPosition(Vector3 entityPosition, Vector3 targetPosition) =>
        Vector3.Distance(entityPosition, targetPosition) <= Mathf.Epsilon;
}
