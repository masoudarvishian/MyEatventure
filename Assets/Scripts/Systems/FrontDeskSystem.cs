using Entitas;
using Entitas.Unity;
using System.Linq;
using UniRx;
using UnityEngine;

public sealed class FrontDeskSystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private CompositeDisposable _compositeDisposable = new();

    public FrontDeskSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    ~FrontDeskSystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
        var frontDeskSpots = GetRestaurantTargetPosition().GetFrontDeskSpots();
        CreateAndLinkFrontDeskEntities(frontDeskSpots);
    }

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => OnClickRestaurantUpgrade()).AddTo(_compositeDisposable);
    }

    private void OnClickRestaurantUpgrade()
    {
        UnlinkAndDestroyAllFrontDesks();
        var frontDeskSpots = GetRestaurantTargetPosition().GetFrontDeskSpots();
        CreateAndLinkFrontDeskEntities(frontDeskSpots);
    }

    private void CreateAndLinkFrontDeskEntities(Transform[] frontDeskSpots)
    {
        for (int i = 0; i < frontDeskSpots.Length; i++)
            CreateAndLinkFrontDeskEntity(frontDeskSpots, i);
    }

    private void UnlinkAndDestroyAllFrontDesks()
    {
        foreach (var e in _contexts.game.GetGroup(GameMatcher.FrontDeskSpot).GetEntities())
        {
            e.visual.gameObject.Unlink();
            e.Destroy();
        }
    }

    private void CreateAndLinkFrontDeskEntity(Transform[] frontDeskSpots, int i)
    {
        var e = _contexts.game.CreateEntity();
        e.isFrontDeskSpot = true;
        e.AddIndex(i);
        e.AddPosition(frontDeskSpots[i].position);
        e.AddVisual(frontDeskSpots[i].gameObject);
        frontDeskSpots[i].gameObject.Link(e);
    }

    private RestaurantTargetPositions GetRestaurantTargetPosition() =>
        _contexts.game.GetGroup(GameMatcher.Restaurant).GetEntities().First().visual.gameObject.GetComponent<RestaurantTargetPositions>();
}
