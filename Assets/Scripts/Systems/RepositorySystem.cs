using Entitas;
using Entitas.Unity;
using System;
using System.Linq;
using UniRx;

public sealed class RepositorySystem : IInitializeSystem
{
    public static IObservable<int> OnCoinChanged => _onCoinChanged;

    private readonly Contexts _contexts;
    private readonly DrinkCoinLevelsPriceSO _drinkCoinLevelsPrice;
    private readonly DummyUI _dummyUI;
    private const int InitialCoinValue = 10;
    private static Subject<int> _onCoinChanged = new Subject<int>();
    private CompositeDisposable _compositeDisposable = new();

    public RepositorySystem(
        Contexts contexts,
        DrinkCoinLevelsPriceSO drinkCoinLevelsPrice,
        DummyUI dummyUI)
    {
        _contexts = contexts;
        _drinkCoinLevelsPrice = drinkCoinLevelsPrice;
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
                    .Subscribe(_ => UpdateEntityWithValue(_drinkCoinLevelsPrice.coinLevels[GetRepositoryEntity().currentDrinkLevel.value].coin))
                    .AddTo(_compositeDisposable);
        DummyUISystem.OnClickUpgrade.Subscribe(_ => OnClickUpgrade()).AddTo(_compositeDisposable);
    }

    private void UpdateEntityWithValue(int value)
    {
        var repositoryEntity = GetRepositoryEntity();
        var currentCoins = repositoryEntity.coin.value;
        var newValue = currentCoins + value;
        repositoryEntity.ReplaceCoin(newValue);
        _onCoinChanged?.OnNext(newValue);
    }

    private GameEntity GetRepositoryEntity() => _contexts.game.GetGroup(GameMatcher.Coin).GetEntities().First();

    private void OnClickUpgrade()
    {
        GetRepositoryEntity().currentDrinkLevel.value++;
        UpdateEntityWithValue(_drinkCoinLevelsPrice.coinLevels[GetRepositoryEntity().currentDrinkLevel.value].upgradeCost * -1);
    }

    private void CreateAndLinkEntity()
    {
        var e = _contexts.game.CreateEntity();
        e.AddCoin(InitialCoinValue);
        e.AddCurrentDrinkLevel(0);
        e.AddCurrentRestaurantLevel(0);
        e.AddVisual(_dummyUI.gameObject);
        _dummyUI.GetCoinText().text = InitialCoinValue.ToString();
        _dummyUI.gameObject.Link(e);
    }
}
