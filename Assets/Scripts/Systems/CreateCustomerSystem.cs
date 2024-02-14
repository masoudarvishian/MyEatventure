using Entitas;
using Entitas.Unity;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public sealed class CreateCustomerSystem : IExecuteSystem
{
    private readonly Contexts _contexts;
    private readonly GameObject _customerPrefab;
    private readonly GameObject _customersParent;
    private readonly Transform _customerSpawnPoint;

    public CreateCustomerSystem(Contexts contexts, GameObject customerPrefab, GameObject customersParent, Transform customerSpawnPoint)
    {
        _contexts = contexts;
        _customerPrefab = customerPrefab;
        _customersParent = customersParent;
        _customerSpawnPoint = customerSpawnPoint;
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject customerObj = InstantiateCustomerPrefab();
            GameEntity customerEntity = CreateCustomerEntity(customerObj);

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