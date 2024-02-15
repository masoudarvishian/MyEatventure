using Entitas;
using Entitas.Unity;
using UnityEngine;

public sealed class CreateChefSystem : IInitializeSystem, IExecuteSystem
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
        InstantiateAndLinkChef();
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InstantiateAndLinkChef();
        }
    }

    private void InstantiateAndLinkChef()
    {
        var newChefObj = GameObject.Instantiate(_chefPrefab);
        newChefObj.transform.position = new Vector3(0, 0.5f, 0);
        CreateAndLinkChefEnitty(newChefObj);
    }

    private void CreateAndLinkChefEnitty(GameObject chefPrefabObj)
    {
        var entity = _contexts.game.CreateEntity();
        entity.isChef = true;
        entity.AddPosition(new Vector3(0, 0.5f, 0));
        entity.AddVisual(chefPrefabObj);
        chefPrefabObj.Link(entity);
    }
}
