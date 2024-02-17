using Entitas;
using Entitas.Unity;
using System;
using System.Linq;
using UniRx;

public sealed class RepositorySystem : IInitializeSystem
{
    public static int CurrentRestaurantLevel { get; private set; }
    public static IObservable<int> OnCoinChanged => _onCoinChanged;

    private readonly Contexts _contexts;
    private readonly DrinkCoinLevelsPriceSO _drinkCoinLevelsPrice;
    private readonly RestaurantLevelsCostSO _restaurantLevelsCost;
    private readonly DummyUI _dummyUI;
    private const int InitialCoinValue = 10;
    private static Subject<int> _onCoinChanged = new Subject<int>();
    private CompositeDisposable _compositeDisposable = new();

    public RepositorySystem(
        Contexts contexts,
        DrinkCoinLevelsPriceSO drinkCoinLevelsPrice,
        RestaurantLevelsCostSO restaurantLevelsCost,
        DummyUI dummyUI)
    {
        _contexts = contexts;
        _drinkCoinLevelsPrice = drinkCoinLevelsPrice;
        _restaurantLevelsCost = restaurantLevelsCost;
        _dummyUI = dummyUI;
    }

    ~RepositorySystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
        CreateAndLinkEntity();
    }

    private void SubscribeToEvents()
    {
        DeliveryOrderSystem.OnOrderIsDelivered
            .Subscribe(_ => UpdateEntityWithValue(GetCurrentDrinkLevel().coin)).AddTo(_compositeDisposable);
        DummyUISystem.OnClickDrinkUpgrade.Subscribe(_ => OnClickDrinkUpgrade()).AddTo(_compositeDisposable);
        DummyUISystem.OnClickRestaurantUpgrade.Subscribe(_ => OnClickRestaurantUpgrade()).AddTo(_compositeDisposable);
    }

    private CoinLevel GetCurrentDrinkLevel() =>
        _drinkCoinLevelsPrice.coinLevels[GetRepositoryEntity().currentDrinkLevel.value];

    private void UpdateEntityWithValue(int value)
    {
        var repositoryEntity = GetRepositoryEntity();
        var currentCoins = repositoryEntity.coin.value;
        var newValue = currentCoins + value;
        repositoryEntity.ReplaceCoin(newValue);
        _onCoinChanged?.OnNext(newValue);
    }

    private GameEntity GetRepositoryEntity() => _contexts.game.GetGroup(GameMatcher.Coin).GetEntities().First();

    private void OnClickDrinkUpgrade()
    {
        GetRepositoryEntity().currentDrinkLevel.value++;
        UpdateEntityWithValue(GetCurrentDrinkLevel().upgradeCost * -1);
    }

    private void OnClickRestaurantUpgrade()
    {
        CurrentRestaurantLevel++;
        UpdateEntityWithValue(GetCurrentRestaurantLevel().upgradeCost * -1);
    }

    private RestaurantLevel GetCurrentRestaurantLevel() =>
        _restaurantLevelsCost.restaurantLevels[CurrentRestaurantLevel];

    private void CreateAndLinkEntity()
    {
        var e = _contexts.game.CreateEntity();
        e.AddCoin(InitialCoinValue);
        e.AddCurrentDrinkLevel(0);
        e.AddVisual(_dummyUI.gameObject);
        _dummyUI.GetCoinText().text = InitialCoinValue.ToString();
        _dummyUI.gameObject.Link(e);
    }
}
