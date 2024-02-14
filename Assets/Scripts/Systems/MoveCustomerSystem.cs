using Entitas;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public sealed class MoveCustomerSystem : IExecuteSystem
{
    private IGroup<GameEntity> _customerGroup;
    private readonly IGroup<GameEntity> _frontDeskGroup;
    private float _speed = 3.0f;
    private readonly Contexts _contexts;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;
    private bool _once = false;

    public MoveCustomerSystem(Contexts contexts, RestaurantTargetPositions restaurantTargetPositions)
    {
        _contexts = contexts;
        _restaurantTargetPositions = restaurantTargetPositions;
        _frontDeskGroup = _contexts.game.GetGroup(GameMatcher.FrontDeskSpot);
        _customerGroup = _contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Customer));
    }

    public void Execute()
    {
        foreach (var entity in _customerGroup.GetEntities().Where(x => x.isCustomer))
        {
            var emptyFrontDeskEntity = _frontDeskGroup.GetEntities().First(x => !x.isOccupied);
            
            MoveEntity(entity, emptyFrontDeskEntity.position.value);

            if (!_once && Vector3.Distance(entity.position.value, emptyFrontDeskEntity.position.value) <= Mathf.Epsilon)
            {
                var freeChefEntity = _contexts.game.GetGroup(GameMatcher.Chef).GetEntities().Where(x => !x.hasCustomerIndex).First();
                freeChefEntity.AddCustomerIndex(entity.creationIndex);

                var behindDeskTargetPos = _restaurantTargetPositions.GetBehindDeskSpots().ElementAt(emptyFrontDeskEntity.index.value).position;
                freeChefEntity.AddTargetPosition(behindDeskTargetPos);
                entity.AddTargetDeskPosition(behindDeskTargetPos);
                
                _once = true;
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
