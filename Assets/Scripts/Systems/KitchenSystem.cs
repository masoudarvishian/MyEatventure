using Entitas;
using Entitas.Unity;
using System.Linq;
using UniRx;
using UnityEngine;

public sealed class KitchenSystem : IInitializeSystem
{
    private Contexts _contexts;
    private IGroup<GameEntity> _restaurantGroup;
    private IGroup<GameEntity> _kitchenGroup;
    private CompositeDisposable _compositeDisposable = new();

    public KitchenSystem(Contexts contexts)
    {
        _contexts = contexts;
        _restaurantGroup = _contexts.game.GetGroup(GameMatcher.Restaurant);
        _kitchenGroup = _contexts.game.GetGroup(GameMatcher.Kitchen);
    }

    ~KitchenSystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
        CreateAndLinkKitchenEntities(GetKitchenSpots());
    }

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => OnClickRestaurantUpgrade()).AddTo(_compositeDisposable);
    }

    private void OnClickRestaurantUpgrade()
    {
        UnlinkAndDestroyAllKitchens();
        CreateAndLinkKitchenEntities(GetKitchenSpots());
    }

    private void UnlinkAndDestroyAllKitchens()
    {
        foreach (var e in _kitchenGroup.GetEntities())
        {
            e.visual.gameObject.Unlink();
            e.Destroy();
        }
    }

    private void CreateAndLinkKitchenEntities(Transform[] kitchenSpots)
    {
        for (int i = 0; i < kitchenSpots.Length; i++)
            CreateAndLinkKitchenEntity(kitchenSpots, i);
    }

    private void CreateAndLinkKitchenEntity(Transform[] kitchenSpots, int index)
    {
        var e = _contexts.game.CreateEntity();
        e.isKitchen = true;
        e.AddIndex(index);
        e.AddPosition(kitchenSpots[index].position);
        e.AddVisual(kitchenSpots[index].gameObject);
        kitchenSpots[index].gameObject.Link(e);
    }

    private Transform[] GetKitchenSpots() =>
        GetRestaurantTargetPosition().GetKitchenSpots();

    private RestaurantTargetPositions GetRestaurantTargetPosition() =>
       _restaurantGroup.GetEntities().First().visual.gameObject.GetComponent<RestaurantTargetPositions>();
}
