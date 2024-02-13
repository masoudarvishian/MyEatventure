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
        var entity = _contexts.game.CreateEntity();
        entity.isChef = true;
        entity.AddPosition(new Vector3(0, 0, 0));
        entity.AddChefVisual(_chefPrefab);
        _chefPrefab.Link(entity);
    }
}
