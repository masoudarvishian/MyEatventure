using Entitas;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _chefPrefab;
    [SerializeField] private CooldownHelper _cooldownHelper;

    private Systems _systems;

    private void Start()
    {
        _systems = new Feature().Add(
            new ChefSystems(Contexts.sharedInstance, _chefPrefab, _cooldownHelper)
        );
        _systems.Initialize();
    }

    private void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
