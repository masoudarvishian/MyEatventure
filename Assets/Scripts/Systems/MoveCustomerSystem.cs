using Entitas;
using Entitas.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class MoveCustomerSystem : IExecuteSystem
{
    private IGroup<GameEntity> _customerGroup;
    private float _speed = 3.0f;
    private readonly Contexts _contexts;
    private readonly Transform _customerLeavingPoint;

    public MoveCustomerSystem(Contexts contexts, Transform customerLeavingPoint)
    {
        _contexts = contexts;
        _customerLeavingPoint = customerLeavingPoint;
        _customerGroup = _contexts.game.GetGroup(GameMatcher.Customer);
    }

    public void Execute()
    {
        MoveCustomersToRestaurant(_customerGroup.GetEntities().Where(x => x.hasTargetPosition));
        LeaveCustomersFromRestaurant(_customerGroup.GetEntities().Where(x => x.delivered.value));
    }

    private void MoveCustomersToRestaurant(IEnumerable<GameEntity> customerEntities)
    {
        foreach (var customerEntity in customerEntities)
        {
            MoveEntity(customerEntity, customerEntity.targetPosition.value);
            if (HasReachedToTargetPosition(customerEntity, customerEntity.targetPosition.value))
                UpdateWaitingRelatedComponent(customerEntity);
        }
    }

    private void MoveEntity(GameEntity entity, Vector3 targetPosition)
    {
        var entityTransform = entity.visual.gameObject.transform;
        entityTransform.position = Vector3.MoveTowards(entityTransform.position, targetPosition, GetStep());
        entity.position.value = entityTransform.position;
    }

    private float GetStep() => _speed * Time.deltaTime;

    private static bool HasReachedToTargetPosition(GameEntity customerEntity, Vector3 targetPosition) =>
        Vector3.Distance(customerEntity.position.value, targetPosition) <= Mathf.Epsilon;

    private static void UpdateWaitingRelatedComponent(GameEntity customerEntity)
    {
        customerEntity.isShowCanvas = true;
        customerEntity.isWaiting = true;
        customerEntity.RemoveTargetPosition();
    }

    private void LeaveCustomersFromRestaurant(IEnumerable<GameEntity> customerEntities)
    {
        foreach (var customerEntity in customerEntities)
        {
            UpdateHidingRelatedComponents(customerEntity);
            MoveEntity(customerEntity, _customerLeavingPoint.position);
            if (HasReachedToTargetPosition(customerEntity, _customerLeavingPoint.position))
                UnlikAndDestroyEntity(customerEntity);
        }
    }

    private static void UpdateHidingRelatedComponents(GameEntity customerEntity)
    {
        if (customerEntity.isShowCanvas)
            customerEntity.isShowCanvas = false;
    }

    private static void UnlikAndDestroyEntity(GameEntity customerEntity)
    {
        customerEntity.visual.gameObject.Unlink();
        GameObject.Destroy(customerEntity.visual.gameObject);
        customerEntity.Destroy();
    }
}
