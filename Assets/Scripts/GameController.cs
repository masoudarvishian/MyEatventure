using Entitas;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _chefPrefab;

    private Systems _systems;

    private void Start()
    {
        _systems = new Feature().Add(
            new ChefSystems(Contexts.sharedInstance, _chefPrefab)
        );
        _systems.Initialize();
    }

    private void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
