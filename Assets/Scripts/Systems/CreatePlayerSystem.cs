using Entitas;

public sealed class CreatePlayerSystem : IInitializeSystem
{
    private Contexts contexts;

    public CreatePlayerSystem(Contexts contexts)
    {
        this.contexts = contexts;
    }

    public void Initialize()
    {
        var e = contexts.game.CreateEntity();
        e.AddHealth(100);
    }
}
