using Entitas;
using Entitas.Unity;
using System.Linq;

public sealed class FrontDeskSystem : IInitializeSystem
{
    private readonly Contexts _contexts;

    public FrontDeskSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Initialize()
    {
        var frontDeskSpots = GetRestaurantTargetPosition().GetFrontDeskSpots();
        for (int i = 0; i < frontDeskSpots.Length; i++)
        {
            var e = _contexts.game.CreateEntity();
            e.isFrontDeskSpot = true;
            e.AddIndex(i);
            e.AddPosition(frontDeskSpots[i].position);
            frontDeskSpots[i].gameObject.Link(e);
        }
    }

    private RestaurantTargetPositions GetRestaurantTargetPosition() =>
        _contexts.game.GetGroup(GameMatcher.Restaurant).GetEntities().First().visual.gameObject.GetComponent<RestaurantTargetPositions>();
}
