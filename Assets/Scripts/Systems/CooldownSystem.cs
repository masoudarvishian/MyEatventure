using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class CooldownSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private readonly CooldownHelper _cooldownHelper;
    private float duration;

    public CooldownSystem(Contexts contexts, CooldownHelper cooldownHelper) : base(contexts.game)
    {
        _contexts = contexts;
        _cooldownHelper = cooldownHelper;
        _cooldownHelper.onTimerIsUp += onCooldownTimerIsUp;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        duration = _contexts.game.GetGroup(GameMatcher.Cooldown).GetEntities().First().cooldown.duration;

        Debug.Log($"cooldown duration {duration}");
        _cooldownHelper.StartCooldownTimer(duration);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasCooldown;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Cooldown).Added());
    }

    private void onCooldownTimerIsUp(object sender, System.EventArgs e)
    {
        Debug.Log("_cooldownHelper_onTimerIsUp");
    }
}
