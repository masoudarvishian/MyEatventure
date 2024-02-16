using Entitas;
using System;
using System.Linq;
using UniRx;

public sealed class DummyUISystem : IInitializeSystem
{
    public static IObservable<Unit> OnClickDrinkUpgrade => _onClickDrinkUpgrade;
    public static IObservable<Unit> OnClickRestaurantUpgrade => _onClickRestaurantUpgrade;

    private readonly Contexts _contexts;
    private readonly DummyUI _dummyUI;
    private readonly DrinkCoinLevelsPriceSO _drinkCoinLevelsPrice;
    private readonly RestaurantLevelsCostSO _restaurantLevelsCost;
    private CompositeDisposable _compositeDisposable = new();
    private static ISubject<Unit> _onClickDrinkUpgrade = new Subject<Unit>();
    private static ISubject<Unit> _onClickRestaurantUpgrade = new Subject<Unit>();

    public DummyUISystem(
        Contexts contexts,
        DummyUI dummyUI,
        DrinkCoinLevelsPriceSO drinkCoinLevelsPrice,
        RestaurantLevelsCostSO restaurantLevelsCost)
    {
        _contexts = contexts;
        _dummyUI = dummyUI;
        _drinkCoinLevelsPrice = drinkCoinLevelsPrice;
        _restaurantLevelsCost = restaurantLevelsCost;
    }

    ~DummyUISystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        RepositorySystem.OnCoinChanged.Subscribe(coinAmount =>
        {
            UpdateCoinUIWithValue(coinAmount);
            HandleDrinkUIUpgrade(coinAmount);
            HandleRestaurantUIUpgrade(coinAmount);
        }).AddTo(_compositeDisposable);
        _dummyUI.GetDrinkUpgradeBtn().OnClickAsObservable().Subscribe(_ => OnDrinkUpgrade()).AddTo(_compositeDisposable);
        _dummyUI.GetRestaurantUpgradeBtn().OnClickAsObservable().Subscribe(_ => OnRestaurantUpgrade()).AddTo(_compositeDisposable);
    }

    private void HandleDrinkUIUpgrade(int coinAmount)
    {
        var currentDrinkCoinLevel = GetRepositoryEntity().currentDrinkLevel.value;
        if (IsAtEndOfDrinkLevel(currentDrinkCoinLevel))
            return;
        HandleDrinkUpgradeInfo(coinAmount, currentDrinkCoinLevel);
    }

    private void HandleRestaurantUIUpgrade(int coinAmount)
    {
        var currentRestaurantLevel = RepositorySystem.CurrentRestaurantLevel;
        if (IsAtEndOfRestaurantLevel(currentRestaurantLevel))
            return;
        HandleRestaurantUpgradeInfo(coinAmount, currentRestaurantLevel);
    }

    private void HandleDrinkUpgradeInfo(int coinAmount, int currentDrinkCoinLevel)
    {
        if (IsEligibleToUpgradeDrink(coinAmount, currentDrinkCoinLevel))
            ShowDrinkUpgradeInfo(currentDrinkCoinLevel);
        else
            HideDrinkUpgradeInfo();
    }

    private void HandleRestaurantUpgradeInfo(int coinAmount, int currentRestaurantLevel)
    {
        if (IsEligibleToUpgradeRestaurant(coinAmount, currentRestaurantLevel))
            ShowRestaurantUpgradeInfo(currentRestaurantLevel);
        else
            HideRestaurantUpgradeInfo();
    }

    private void UpdateCoinUIWithValue(int value)
    {
        GetRepositoryEntity().visual.gameObject.GetComponent<DummyUI>().GetCoinText().text = value.ToString();
    }

    private GameEntity GetRepositoryEntity() => _contexts.game.GetGroup(GameMatcher.Coin).GetEntities().First();

    private bool IsAtEndOfDrinkLevel(int currentDrinkCoinLevel) =>
        currentDrinkCoinLevel == _drinkCoinLevelsPrice.coinLevels.Length - 1;

    private bool IsAtEndOfRestaurantLevel(int currentRestaurantLevel) =>
        currentRestaurantLevel == _restaurantLevelsCost.restaurantLevels.Length - 1;

    private bool IsEligibleToUpgradeDrink(int coinAmount, int currentDrinkCoinLevel) =>
       coinAmount >= _drinkCoinLevelsPrice.coinLevels[currentDrinkCoinLevel + 1].upgradeCost;

    private bool IsEligibleToUpgradeRestaurant(int coinAmount, int currentRestaurantLevel) =>
        coinAmount >= _restaurantLevelsCost.restaurantLevels[currentRestaurantLevel + 1].upgradeCost;

    private void ShowDrinkUpgradeInfo(int currentDrinkCoinLevel)
    {
        _dummyUI.GetUpgradeDrinkCostText().text = _drinkCoinLevelsPrice.coinLevels[currentDrinkCoinLevel + 1].upgradeCost.ToString();
        _dummyUI.GetDrinkPriceText().text = _drinkCoinLevelsPrice.coinLevels[currentDrinkCoinLevel + 1].coin.ToString();
        _dummyUI.GetDrinkUpgradeInfo().SetActive(true);
    }

    private void ShowRestaurantUpgradeInfo(int currentRestaurantLevel)
    {
        _dummyUI.GetUpgradeRestaurantCostText().text = _restaurantLevelsCost.restaurantLevels[currentRestaurantLevel + 1].upgradeCost.ToString();
        _dummyUI.GetRestaurantUpgradeInfo().SetActive(true);
    }

    private void OnDrinkUpgrade()
    {
        HideDrinkUpgradeInfo();
        _onClickDrinkUpgrade?.OnNext(Unit.Default);
    }

    private void OnRestaurantUpgrade()
    {
        HideRestaurantUpgradeInfo();
        _onClickRestaurantUpgrade?.OnNext(Unit.Default);
    }

    private void HideDrinkUpgradeInfo()
    {
        _dummyUI.GetDrinkUpgradeInfo().SetActive(false);
    }

    private void HideRestaurantUpgradeInfo()
    {
        _dummyUI.GetRestaurantUpgradeInfo().SetActive(false);
    }
}
