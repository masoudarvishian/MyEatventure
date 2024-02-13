using Entitas;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class CooldownSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private float duration;

    public CooldownSystem(Contexts contexts) : base(contexts.game)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        duration = _contexts.game.GetGroup(GameMatcher.Cooldown).GetEntities().First().cooldown.duration;

        Debug.Log($"cooldown duration {duration}");
        foreach (var entity in entities)
        {
            

            Debug.Log("Time is up!");
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasCooldown;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Cooldown).Added());
    }
}
