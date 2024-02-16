using Entitas;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private DummyUI _dummyUI;
    [SerializeField] private GameObject _chefPrefab;

    [SerializeField] private GameObject _customersPrefab;
    [SerializeField] private GameObject _customersParent;
    [SerializeField] private Transform _customerSpawnPoint;
    [SerializeField] private Transform _customerLeavingPoint;

    [SerializeField] private DrinkCoinLevelsPriceSO drinkCoinLevelsPrice;
    [SerializeField] private RestaurantLevelsCostSO restaurantLevelsCost;

    private Systems _systems;

    private void Start()
    {
        _systems = new Feature().Add(
            new RootSystems(
                Contexts.sharedInstance,
                _chefPrefab,
                _customersPrefab,
                _customersParent,
                _customerSpawnPoint,
                _customerLeavingPoint,
                _dummyUI,
                drinkCoinLevelsPrice,
                restaurantLevelsCost)
        );
        _systems.Initialize();
    }

    private void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
