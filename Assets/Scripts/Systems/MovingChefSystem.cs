using Entitas;
using UnityEngine;

internal class MovingChefSystem : IExecuteSystem
{
    private readonly Contexts _contexts;
    private float speed = 2.0f;

    public MovingChefSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        foreach (var e in _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef).AnyOf(GameMatcher.Mover)).GetEntities())
        {
            var entityTransform = e.chefVisual.gameObject.transform;
            var step = speed * Time.deltaTime;
            entityTransform.position = Vector3.MoveTowards(entityTransform.position, e.targetPosition.value, step);
            e.position.value = entityTransform.position;

            if (Vector3.Distance(entityTransform.position, e.targetPosition.value) <= Mathf.Epsilon)
            {
                e.isMover = false;
                e.RemoveTargetPosition();
            }
        }
    }
}
