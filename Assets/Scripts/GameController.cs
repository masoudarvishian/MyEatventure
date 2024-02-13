using Entitas;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _chefPrefab;
    [SerializeField] private CooldownHelper _cooldownHelper;
    [SerializeField] private RestaurantTargetPositions _restaurantTargetPositions;

    private Systems _systems;

    private void Start()
    {
        _systems = new Feature().Add(
            new ChefSystems(Contexts.sharedInstance, _chefPrefab, _cooldownHelper, _restaurantTargetPositions)
        );
        _systems.Initialize();
    }

    private void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
