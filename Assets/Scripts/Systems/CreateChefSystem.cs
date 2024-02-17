using Entitas;
using Entitas.Unity;
using UniRx;
using UnityEngine;

public sealed class CreateChefSystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly GameObject _chefPrefab;
    private readonly IGroup<GameEntity> _chefGroup;
    private readonly CompositeDisposable _compositeDisposable = new();

    public CreateChefSystem(Contexts contexts, GameObject chefPrefab)
    {
        _contexts = contexts;
        _chefPrefab = chefPrefab;
        _chefGroup = _contexts.game.GetGroup(GameMatcher.Chef);
    }

    ~CreateChefSystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
        InstantiateAndLinkChef();
    }

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickAddChef.Subscribe(_ => InstantiateAndLinkChef()).AddTo(_compositeDisposable);
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => ResetChefPositions()).AddTo(_compositeDisposable);
    }

    private void ResetChefPositions()
    {
        var chefEntitiesLength = _chefGroup.GetEntities().Length;
        for (int i = 0; i < chefEntitiesLength; i++)
        {
            GameEntity chefEntity = _chefGroup.GetEntities()[i];
            chefEntity.visual.gameObject.transform.position = new Vector3((i * chefEntitiesLength) - chefEntitiesLength, 0.5f, 0f);
            chefEntity.position.value = new Vector3((i * chefEntitiesLength) - chefEntitiesLength, 0.5f, 0f);
            if (chefEntity.hasTargetPosition)
                chefEntity.RemoveTargetPosition();
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
