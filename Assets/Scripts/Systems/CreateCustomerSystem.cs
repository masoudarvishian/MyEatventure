using Entitas;
using Entitas.Unity;
using System.Linq;
using UnityEngine;

public sealed class CreateCustomerSystem : IExecuteSystem
{
    private readonly Contexts _contexts;
    private readonly GameObject _customerPrefab;
    private readonly GameObject _customersParent;
    private readonly Transform _customerSpawnPoint;
    private readonly RestaurantTargetPositions _restaurantTargetPositions;
    private readonly IGroup<GameEntity> _frontDeskGroup;

    public CreateCustomerSystem(Contexts contexts, GameObject customerPrefab, GameObject customersParent, Transform customerSpawnPoint,
        RestaurantTargetPositions restaurantTargetPositions)
    {
        _contexts = contexts;
        _customerPrefab = customerPrefab;
        _customersParent = customersParent;
        _customerSpawnPoint = customerSpawnPoint;
        _restaurantTargetPositions = restaurantTargetPositions;
        _frontDeskGroup = _contexts.game.GetGroup(GameMatcher.FrontDeskSpot);
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject customerObj = InstantiateCustomerPrefab();
            GameEntity customerEntity = CreateCustomerEntity(customerObj);

            var emptyFrontDeskEntity = _frontDeskGroup.GetEntities().First(x => !x.isOccupied);
            var behindDeskTargetPos = _restaurantTargetPositions.GetBehindDeskSpots().ElementAt(emptyFrontDeskEntity.index.value).position;
            
            customerEntity.AddTargetDeskPosition(behindDeskTargetPos);
            customerEntity.AddTargetPosition(emptyFrontDeskEntity.position.value);
            customerEntity.AddTargetDeskIndex(emptyFrontDeskEntity.index.value);
            emptyFrontDeskEntity.isOccupied = true;

            customerObj.Link(customerEntity);
        }
    }

    private GameObject InstantiateCustomerPrefab()
    {
        var customerObj = GameObject.Instantiate(_customerPrefab);
        customerObj.transform.SetParent(_customersParent.transform);
        customerObj.transform.position = _customerSpawnPoint.position;
        return customerObj;
    }

    private GameEntity CreateCustomerEntity(GameObject customerObj)
    {
        var entity = _contexts.game.CreateEntity();
        entity.isCustomer = true;
        entity.AddPosition(customerObj.transform.position);
        entity.isPreparingOrder = false;
        entity.AddDelivered(false);
        entity.AddQuantity(2);
        entity.AddVisual(customerObj);
        return entity;
    }
}