using Entitas;
using UnityEngine;

internal class MovingChefSystem : IExecuteSystem
{
    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _chefWithTargetPositionGroup;
    private float _speed = 4.0f;

    public MovingChefSystem(Contexts contexts)
    {
        _contexts = contexts;
        _chefWithTargetPositionGroup = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef).AnyOf(GameMatcher.TargetPosition));
    }

    public void Execute()
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
