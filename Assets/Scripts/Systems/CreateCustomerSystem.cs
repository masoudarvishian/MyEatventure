using Entitas;
using UnityEngine;

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
            var customerObj = GameObject.Instantiate(_customerPrefab);
            customerObj.transform.SetParent(_customersParent.transform);
            customerObj.transform.position = _customerSpawnPoint.position;
        }
    }
}
