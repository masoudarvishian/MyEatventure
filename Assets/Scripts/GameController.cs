using Entitas;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Systems _systems;

    private void Start()
    {
        _systems = new Feature("My Features").Add(
            new RootSystems(Contexts.sharedInstance)
        );

        _systems.Initialize();
    }

    private void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
