using Entitas;
using System.Linq;
using UnityEngine;

public sealed class TargetPositionEmitterSystem : IExecuteSystem
{
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;
    private readonly IGroup<GameEntity> _waitingCustomerGroup;

    public TargetPositionEmitterSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
        _waitingCustomerGroup = _contexts.game.GetGroup(GameMatcher.WaitingCustomer);
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();
            entity.AddWaitingCustomer(_restaurantTargetPositions.GetFirstCustomerSpot().position);
            entity.isPreparingOrder = false;
            entity.AddDelivered(false);
            entity.AddQuantity(2);
            var freeChefEntity = _contexts.game.GetGroup(GameMatcher.Chef).GetEntities().Where(x => !x.hasCustomerIndex).First();
            freeChefEntity.AddCustomerIndex(entity.creationIndex);

            var e = _contexts.game.CreateEntity();
            e.AddTargetPosition(_restaurantTargetPositions.GetFirstCustomerSpot().position);
        }
    }
}
