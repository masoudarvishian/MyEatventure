using Entitas;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private GameObject _chefPrefab;
    [SerializeField] private RestaurantTargetPositions _restaurantTargetPositions;

    [SerializeField] private GameObject _customersPrefab;
    [SerializeField] private GameObject _customersParent;
    [SerializeField] private Transform _customerSpawnPoint;
    [SerializeField] private Transform _customerLeavingPoint;

    private Systems _systems;

    private void Start()
    {
        _systems = new Feature().Add(
            new RootSystems(
                Contexts.sharedInstance, 
                _chefPrefab, 
                _restaurantTargetPositions,
                _customersPrefab, 
                _customersParent, 
                _customerSpawnPoint, 
                _customerLeavingPoint,
                _coinText)
        );
        _systems.Initialize();
    }

    private void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
