using Entitas;

public sealed class RepositorySystem : IInitializeSystem
{
    private readonly Contexts _contexts;

    public RepositorySystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        var e = _contexts.game.CreateEntity();
        e.AddCoin(10);
    }
}
