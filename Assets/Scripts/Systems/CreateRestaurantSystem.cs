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
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => OnClickRestaurantUpgrade()).AddTo(_compositeDisposable);

        var restaurantObj = GameObject.Instantiate(_restaurantLevelsCost.restaurantLevels[RepositorySystem.CurrentRestaurantLevel].prefab);
        restaurantObj.transform.position = Vector3.zero;

        var e = _contexts.game.CreateEntity();
        e.isRestaurant = true;
        e.AddVisual(restaurantObj);
        restaurantObj.Link(e);
    }

    private  void OnClickRestaurantUpgrade()
    {
        var prevRestaurantObj = GameObject.FindAnyObjectByType<RestaurantTargetPositions>().gameObject;
        prevRestaurantObj.Unlink();
        
        var restaurantObj = GameObject.Instantiate(_restaurantLevelsCost.restaurantLevels[RepositorySystem.CurrentRestaurantLevel].prefab);
        restaurantObj.transform.position = Vector3.zero;

        var restaurantEntity = _contexts.game.GetGroup(GameMatcher.Restaurant).GetEntities().First();
        restaurantEntity.visual.gameObject = restaurantObj;

        restaurantObj.Link(restaurantEntity);

       GameObject.Destroy(prevRestaurantObj);
    }
}
