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
        foreach (var e in _contexts.game.GetGroup(GameMatcher.ChefVisual).GetEntities())
        {
            var entityTransform = e.chefVisual.gameObject.transform;
            Vector3 targetPosition = new Vector3(7f, 0, 0);
            var step = speed * Time.deltaTime;
            entityTransform.position = Vector3.MoveTowards(entityTransform.position, targetPosition, step);
            e.position.value = entityTransform.position;
        }
    }
}
