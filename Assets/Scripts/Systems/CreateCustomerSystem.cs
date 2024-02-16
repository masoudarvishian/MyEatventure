using Entitas;
using Entitas.Unity;
using System.Linq;
using UnityEngine;
using UniRx;
using System;

public sealed class CreateCustomerSystem : IExecuteSystem, IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly GameObject _customerPrefab;
    private readonly GameObject _customersParent;
    private readonly Transform _customerSpawnPoint;
    private readonly IGroup<GameEntity> _frontDeskGroup;
    private readonly CompositeDisposable _compositeDisposable = new();

    private int minGenerateInterval = 5;
    private int maxGenerateInterval = 10;

    public CreateCustomerSystem(
        Contexts contexts, 
        GameObject customerPrefab, 
        GameObject customersParent, 
        Transform customerSpawnPoint)
    {
        _contexts = contexts;
        _customerPrefab = customerPrefab;
        _customersParent = customersParent;
        _customerSpawnPoint = customerSpawnPoint;
       
        _frontDeskGroup = _contexts.game.GetGroup(GameMatcher.FrontDeskSpot);
    }

    ~CreateCustomerSystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => OnClickRestaurantUpgrade()).AddTo(_compositeDisposable);

        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => GenerateCustomer()).AddTo(_compositeDisposable);

        Observable.Interval(TimeSpan.FromSeconds(UnityEngine.Random.Range(minGenerateInterval, maxGenerateInterval))).Subscribe(_ =>
        {
            GenerateCustomer();
        }).AddTo(_compositeDisposable);
    }

    private void OnClickRestaurantUpgrade()
    {
        minGenerateInterval = 2;
        maxGenerateInterval = 5;

        foreach(var e in _contexts.game.GetGroup(GameMatcher.Customer).GetEntities())
        {
            var obj = e.visual.gameObject;
            obj.Unlink();
            e.Destroy();
            GameObject.Destroy(obj);
        }
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.C))
            GenerateCustomer();
    }

    private void GenerateCustomer()
    {
        var restaurantTargetPositions = GetRestaurantTargetPosition();
        var occupiedDeskCount = _frontDeskGroup.GetEntities().Where(x => x.isOccupied).Count();
        if (occupiedDeskCount >= restaurantTargetPositions.GetFrontDeskSpots().Count())
            return;

        GameObject customerObj = InstantiateCustomerPrefab();
        GameEntity customerEntity = CreateCustomerEntity(customerObj);

        var emptyFrontDeskEntity = _frontDeskGroup.GetEntities().First(x => !x.isOccupied);
        var behindDeskTargetPos = restaurantTargetPositions.GetBehindDeskSpots().ElementAt(emptyFrontDeskEntity.index.value).position;

        customerEntity.AddTargetDeskPosition(behindDeskTargetPos);
        customerEntity.AddTargetPosition(emptyFrontDeskEntity.position.value);
        customerEntity.AddTargetDeskIndex(emptyFrontDeskEntity.index.value);
        emptyFrontDeskEntity.isOccupied = true;

        customerObj.Link(customerEntity);
    }

    private GameObject InstantiateCustomerPrefab()
    {
        var customerObj = GameObject.Instantiate(_customerPrefab);
        customerObj.transform.SetParent(_customersParent.transform);
        customerObj.transform.position = _customerSpawnPoint.position;
        return customerObj;
    }

    private GameEntity CreateCustomerEntity(GameObject customerObj)
    {
        var entity = _contexts.game.CreateEntity();
        entity.isCustomer = true;
        entity.AddPosition(customerObj.transform.position);
        entity.isPreparingOrder = false;
        entity.AddDelivered(false);
        entity.AddQuantity(UnityEngine.Random.Range(1, 3));
        entity.AddVisual(customerObj);
        return entity;
    }

    private RestaurantTargetPositions GetRestaurantTargetPosition() =>
        _contexts.game.GetGroup(GameMatcher.Restaurant).GetEntities().First().visual.gameObject.GetComponent<RestaurantTargetPositions>();
}