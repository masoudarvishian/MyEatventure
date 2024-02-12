using Entitas;
using System.Collections.Generic;

public sealed class TargetPositionReacterSystem : ReactiveSystem<GameEntity>
{
    private IGroup<GameEntity> _movers;

    public TargetPositionReacterSystem(Contexts contexts) : base(contexts.game)
    {
        _movers = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef).NoneOf(GameMatcher.Mover));
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var e in entities)
        {
            foreach(var mover in _movers.GetEntities())
            {
                mover.isMover = true;
                mover.AddTargetPosition(e.targetPosition.value);
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasTargetPosition;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.TargetPosition);
    }
}
