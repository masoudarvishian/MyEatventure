using Entitas;
using System.Collections.Generic;
using UnityEngine;

public sealed class SetChefAsMoverSystem : ReactiveSystem<GameEntity>
{
    private IGroup<GameEntity> _moversGroup;

    public SetChefAsMoverSystem(Contexts contexts) : base(contexts.game)
    {
        _moversGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef).NoneOf(GameMatcher.Mover));
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
            AddMovingComponentsForMovers(entity.targetPosition.value);
    }

    protected override bool Filter(GameEntity entity) => entity.hasTargetPosition;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.TargetPosition);

    private void AddMovingComponentsForMovers(Vector3 targetPosition)
    {
        foreach (var entity in _moversGroup.GetEntities())
        {
            entity.isMover = true;
            entity.AddTargetPosition(targetPosition);
        }
    }
}
