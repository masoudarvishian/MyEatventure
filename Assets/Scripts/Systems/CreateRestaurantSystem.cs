using Entitas;
using Entitas.Unity;
using System.Linq;
using UniRx;
using UnityEngine;

public sealed class CreateRestaurantSystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly RestaurantLevelsCostSO _restaurantLevelsCost;
    private CompositeDisposable _compositeDisposable = new();

    public CreateRestaurantSystem(Contexts contexts, RestaurantLevelsCostSO restaurantLevelsCost)
    {
        _contexts = contexts;
        _restaurantLevelsCost = restaurantLevelsCost;
    }

    ~CreateRestaurantSystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
        CreateAndLinkRestaurantEntity();
    }

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => OnClickRestaurantUpgrade()).AddTo(_compositeDisposable);
    }

    private void CreateAndLinkRestaurantEntity()
    {
        var restaurantObj = GameObject.Instantiate(GetCurrentRestaurantLevelPrefab());
        var e = _contexts.game.CreateEntity();
        e.isRestaurant = true;
        e.AddVisual(restaurantObj);
        restaurantObj.Link(e);
    }

    private  void OnClickRestaurantUpgrade()
    {
        var prevRestaurantObj = GetRestaurantGameObject();
        prevRestaurantObj.Unlink();
        InstantiateRestaurantAndLinkItTo(GetRestaurantEntity());
        GameObject.Destroy(prevRestaurantObj);
    }

    private static GameObject GetRestaurantGameObject() =>
        GameObject.FindAnyObjectByType<RestaurantTargetPositions>().gameObject;

    private void InstantiateRestaurantAndLinkItTo(GameEntity restaurantEntity)
    {
        var newRestaurantObj = GameObject.Instantiate(GetCurrentRestaurantLevelPrefab());
        restaurantEntity.visual.gameObject = newRestaurantObj;
        newRestaurantObj.Link(restaurantEntity);
    }

    private GameObject GetCurrentRestaurantLevelPrefab() =>
        _restaurantLevelsCost.restaurantLevels[RepositorySystem.CurrentRestaurantLevel].prefab;

    private GameEntity GetRestaurantEntity() =>
        _contexts.game.GetGroup(GameMatcher.Restaurant).GetEntities().First();
}
