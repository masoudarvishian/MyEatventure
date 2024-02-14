using Entitas;
using System.Linq;
using UnityEngine;

public sealed class MoveCustomerSystem : IExecuteSystem
{
    private IGroup<GameEntity> _customerGroup;
    private IGroup<GameEntity> _chefGroup;
    private readonly IGroup<GameEntity> _frontDeskGroup;
    private float _speed = 3.0f;
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;

    public MoveCustomerSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
        _frontDeskGroup = _contexts.game.GetGroup(GameMatcher.FrontDeskSpot);
        _customerGroup = _contexts.game.GetGroup(GameMatcher.Customer);
        _chefGroup = _contexts.game.GetGroup(GameMatcher.Chef);
    }

    public void Execute()
    {
        foreach (var customerEntity in _customerGroup.GetEntities().Where(x => x.hasTargetPosition))
        {
            MoveEntity(customerEntity, customerEntity.targetPosition.value);

            if (Vector3.Distance(customerEntity.position.value, customerEntity.targetPosition.value) <= Mathf.Epsilon)
            {
                customerEntity.RemoveTargetPosition();
                var deskEntity = _frontDeskGroup.GetEntities().First(x => x.index.value == customerEntity.targetDeskIndex.value);
                deskEntity.isOccupied = true;

                var freeChefEntity = _chefGroup.GetEntities().First(x => !x.hasCustomerIndex);
                freeChefEntity.AddCustomerIndex(customerEntity.creationIndex);
                freeChefEntity.AddTargetPosition(customerEntity.targetDeskPosition.value);
            }
        }
    }

    private void MoveEntity(GameEntity entity, Vector3 targetPosition)
    {
        var entityTransform = entity.visual.gameObject.transform;
        entityTransform.position = Vector3.MoveTowards(entityTransform.position, targetPosition, GetStep());
        entity.position.value = entityTransform.position;
    }

    private float GetStep() => _speed * Time.deltaTime;
}
