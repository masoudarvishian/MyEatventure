using Entitas;
using System.Collections.Generic;

public sealed class PreparingOrderDetectorSystem : ReactiveSystem<GameEntity>
{
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;

    public PreparingOrderDetectorSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions) : base(contexts.game)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        var e = _contexts.game.CreateEntity();
        e.AddTargetPosition(_restaurantTargetPositions.GetFirstRestaurantSpot().position);
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Cooldown).Removed());
    }
}
