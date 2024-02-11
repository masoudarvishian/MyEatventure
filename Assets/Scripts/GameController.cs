using UnityEngine;

public class GameController : MonoBehaviour
{
    private RootSystems _rootSystems;

    private void Start()
    {
        _rootSystems = new RootSystems(Contexts.sharedInstance);
        _rootSystems.Initialize();
    }

    private void Update()
    {
        _rootSystems.Execute();
    }
}
