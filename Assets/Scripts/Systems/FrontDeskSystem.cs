using Entitas;
using Entitas.Unity;
using System.Linq;
using UniRx;
using UnityEngine;

public sealed class FrontDeskSystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly IGroup<GameEntity> _frontDeskGroup;
    private readonly IGroup<GameEntity> _restaurantGroup;
    private CompositeDisposable _compositeDisposable = new();

    public FrontDeskSystem(Contexts contexts)
    {
        _contexts = contexts;
        _frontDeskGroup = _contexts.game.GetGroup(GameMatcher.FrontDeskSpot);
        _restaurantGroup = _contexts.game.GetGroup(GameMatcher.Restaurant);
    }

    ~FrontDeskSystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
        CreateAndLinkFrontDeskEntities(GetFrontDeskSpots());
    }

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => OnClickRestaurantUpgrade()).AddTo(_compositeDisposable);
    }

    private void CreateAndLinkFrontDeskEntities(Transform[] frontDeskSpots)
    {
        for (int i = 0; i < frontDeskSpots.Length; i++)
            CreateAndLinkFrontDeskEntity(frontDeskSpots, i);
    }

    private void CreateAndLinkFrontDeskEntity(Transform[] frontDeskSpots, int index)
    {
        var e = _contexts.game.CreateEntity();
        e.isFrontDeskSpot = true;
        e.AddIndex(index);
        e.AddPosition(frontDeskSpots[index].position);
        e.AddVisual(frontDeskSpots[index].gameObject);
        frontDeskSpots[index].gameObject.Link(e);
    }

    private void OnClickRestaurantUpgrade()
    {
        UnlinkAndDestroyAllFrontDesks();
        CreateAndLinkFrontDeskEntities(GetFrontDeskSpots());
    }

    private void UnlinkAndDestroyAllFrontDesks()
    {
        foreach (var e in _frontDeskGroup.GetEntities())
        {
            e.visual.gameObject.Unlink();
            e.Destroy();
        }
    }

    private Transform[] GetFrontDeskSpots() =>
        GetRestaurantTargetPosition().GetFrontDeskSpots();

    private RestaurantTargetPositions GetRestaurantTargetPosition() =>
        _restaurantGroup.GetEntities().First().visual.gameObject.GetComponent<RestaurantTargetPositions>();
}
