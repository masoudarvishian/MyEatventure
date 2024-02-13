using Entitas;

public sealed class CleanUpTargetPositionSystem : ICleanupSystem
{
    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _targetPositionGroup;

    public CleanUpTargetPositionSystem(Contexts contexts)
    {
        _contexts = contexts;
        _targetPositionGroup = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.TargetPosition).NoneOf(GameMatcher.Chef));
    }

    public void Cleanup()
    {
        foreach (var entity in _targetPositionGroup.GetEntities())
            entity.Destroy();
    }
}
