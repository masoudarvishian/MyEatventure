using Entitas;
using UnityEngine;

public sealed class TargetPositionEmitterSystem : IExecuteSystem
{
    private readonly Contexts _contexts;

    public TargetPositionEmitterSystem(Contexts contexts)
    {
        _contexts = contexts;
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var e = _contexts.game.CreateEntity();
            e.AddTargetPosition(new Vector3(Random.Range(-10f, 10f), 0, 0));
        }
    }
}
