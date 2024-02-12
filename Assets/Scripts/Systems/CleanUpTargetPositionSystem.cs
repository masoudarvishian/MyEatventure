using Entitas;

public sealed class CleanUpTargetPositionSystem : ICleanupSystem
{
    private readonly Contexts _contexts;

    public CleanUpTargetPositionSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Cleanup()
    {
        foreach (var e in _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.TargetPosition).NoneOf(GameMatcher.Chef)).GetEntities())
            e.Destroy();
    }
}
