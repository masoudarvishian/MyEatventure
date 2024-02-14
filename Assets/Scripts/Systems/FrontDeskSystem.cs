using Entitas;
using Entitas.Unity;

public sealed class FrontDeskSystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;

    public FrontDeskSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
    }

    public void Initialize()
    {
        var frontDeskSpots = _restaurantTargetPositions.GetFrontDeskSpots();
        for (int i = 0; i < frontDeskSpots.Length; i++)
        {
            var e = _contexts.game.CreateEntity();
            e.isFrontDeskSpot = true;
            e.AddIndex(i);
            e.AddPosition(frontDeskSpots[i].position);
            frontDeskSpots[i].gameObject.Link(e);
        }
    }
}
