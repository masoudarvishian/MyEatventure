using Entitas;
using System.Collections.Generic;
using UnityEngine;

public sealed class SetChefAsMoverSystem : ReactiveSystem<GameEntity>
{
    private IGroup<GameEntity> _chefGroup;

    public SetChefAsMoverSystem(Contexts contexts) : base(contexts.game)
    {
        _chefGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef).NoneOf(GameMatcher.TargetPosition));
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            AddMovingComponentsForChefs(entity.targetPosition.value);
        }
    }

    protected override bool Filter(GameEntity entity) => entity.hasTargetPosition;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.TargetPosition).Added());

    private void AddMovingComponentsForChefs(Vector3 targetPosition)
    {
        foreach (var chefEntity in _chefGroup.GetEntities())
        {
            chefEntity.ReplaceTargetPosition(targetPosition);
        }
    }
}
