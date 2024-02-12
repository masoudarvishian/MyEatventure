using Entitas;
using Entitas.Unity;
using UnityEngine;

public sealed class CreateChefSystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly GameObject _chefPrefab;

    public CreateChefSystem(Contexts contexts, GameObject chefPrefab)
    {
        _contexts = contexts;
        _chefPrefab = chefPrefab;
    }

    public void Initialize()
    {
        var e = _contexts.game.CreateEntity();
        e.isChef = true;
        e.AddPosition(new Vector3(0, 0, 0));
        e.AddChefVisual(_chefPrefab);
        _chefPrefab.Link(e);
    }
}
