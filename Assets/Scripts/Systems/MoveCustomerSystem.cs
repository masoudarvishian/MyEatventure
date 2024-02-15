using Entitas;
using Entitas.Unity;
using System.Linq;
using UnityEngine;

public sealed class MoveCustomerSystem : IExecuteSystem
{
    private IGroup<GameEntity> _customerGroup;
    private IGroup<GameEntity> _chefGroup;
    private float _speed = 3.0f;
    private readonly Contexts _contexts;
    private readonly Transform _customerLeavingPoint;

    public MoveCustomerSystem(Contexts contexts, Transform customerLeavingPoint)
    {
        _contexts = contexts;
        _customerLeavingPoint = customerLeavingPoint;
        _customerGroup = _contexts.game.GetGroup(GameMatcher.Customer);
        _chefGroup = _contexts.game.GetGroup(GameMatcher.Chef);
    }

    public void Execute()
    {
        // move toward the restaurant
        foreach (var customerEntity in _customerGroup.GetEntities().Where(x => x.hasTargetPosition))
        {
            MoveEntity(customerEntity, customerEntity.targetPosition.value);

            if (Vector3.Distance(customerEntity.position.value, customerEntity.targetPosition.value) <= Mathf.Epsilon)
            {
                customerEntity.isShowCanvas = true;
                customerEntity.isWaiting = true;
                
                customerEntity.RemoveTargetPosition();
            }
        }

        // leave the restaurant
        foreach (var customerEntity in _customerGroup.GetEntities().Where(x => x.delivered.value))
        {
            if (customerEntity.isShowCanvas)
                customerEntity.isShowCanvas = false;

            MoveEntity(customerEntity, _customerLeavingPoint.position);
            if (Vector3.Distance(customerEntity.position.value, _customerLeavingPoint.position) <= Mathf.Epsilon)
            {
                customerEntity.visual.gameObject.Unlink();
                GameObject.Destroy(customerEntity.visual.gameObject);
                customerEntity.Destroy();
            }
        }
    }

    private void MoveEntity(GameEntity entity, Vector3 targetPosition)
    {
        var entityTransform = entity.visual.gameObject.transform;
        entityTransform.position = Vector3.MoveTowards(entityTransform.position, targetPosition, GetStep());
        entity.position.value = entityTransform.position;
    }

    private float GetStep() => _speed * Time.deltaTime;
}
