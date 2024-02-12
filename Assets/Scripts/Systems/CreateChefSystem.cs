using Entitas;

public sealed class CreateChefSystem : IInitializeSystem
{
    private readonly Contexts contexts;

    public CreateChefSystem(Contexts contexts)
    {
        this.contexts = contexts;
    }

    public void Initialize()
    {
        var e = contexts.game.CreateEntity();
        e.isChef = true;
        e.AddPosition(new UnityEngine.Vector3(0, 0, 0));
    }
}
