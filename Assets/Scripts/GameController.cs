using Entitas;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _chefPrefab;
    [SerializeField] private CooldownHelper _cooldownHelper;
    [SerializeField] private RestaurantTargetPositions _restaurantTargetPositions;

    [SerializeField] private GameObject _customersPrefab;
    [SerializeField] private GameObject _customersParent;
    [SerializeField] private Transform _customerSpawnPoint;
    [SerializeField] private Transform _customerLeavingPoint;

    private Systems _systems;

    private void Start()
    {
        _systems = new Feature().Add(
            new ChefSystems(Contexts.sharedInstance, _chefPrefab, _cooldownHelper, _restaurantTargetPositions,
            _customersPrefab, _customersParent, _customerSpawnPoint, _customerLeavingPoint)
        );
        _systems.Initialize();
    }

    private void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
