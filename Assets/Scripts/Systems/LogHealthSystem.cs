using Entitas;
using System.Collections.Generic;
using UnityEngine;

public sealed class LogHealthSystem : ReactiveSystem<GameEntity>
{
    public LogHealthSystem(Contexts contexts) : base(contexts.game) { }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
            Debug.Log($"Health: {e.health.value}");
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasHealth;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Health);
    }
}
