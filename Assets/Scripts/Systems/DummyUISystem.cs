using Entitas;
using System;
using System.Linq;
using UniRx;

public sealed class DummyUISystem : IInitializeSystem
{
    public static IObservable<Unit> OnClickUpgrade => _onClickUpgrade;

    private readonly Contexts _contexts;
    private readonly DummyUI _dummyUI;
    private readonly DrinkCoinLevelsPriceSO _drinkCoinLevelsPrice;
    private CompositeDisposable _compositeDisposable = new();
    private static ISubject<Unit> _onClickUpgrade = new Subject<Unit>();

    public DummyUISystem(
        Contexts contexts, 
        DummyUI dummyUI, 
        DrinkCoinLevelsPriceSO drinkCoinLevelsPrice)
    {
        _contexts = contexts;
        _dummyUI = dummyUI;
        _drinkCoinLevelsPrice = drinkCoinLevelsPrice;
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
        RepositorySystem.OnCoinChanged.Subscribe(OnCoinAmountChanged).AddTo(_compositeDisposable);
        _dummyUI.GetUpgradeBtn().OnClickAsObservable().Subscribe(_ => OnUpgrade()).AddTo(_compositeDisposable);
    }

    private void OnCoinAmountChanged(int coinAmount)
    {
        UpdateCoinUIWithValue(coinAmount);

        var currentDrinkCoinLevel = GetRepositoryEntity().currentDrinkLevel.value;
        if (IsAtEndOfDrinkLevel(currentDrinkCoinLevel))
            return;

        HandleUpgradeInfo(coinAmount, currentDrinkCoinLevel);
    }

    private void HandleUpgradeInfo(int coinAmount, int currentDrinkCoinLevel)
    {
        if (IsEligibleToUpgradeDrink(coinAmount, currentDrinkCoinLevel))
            ShowUpgradeInfo(currentDrinkCoinLevel);
        else
            HideUpgradeInfo();
    }

    private void UpdateCoinUIWithValue(int value)
    {
        GetRepositoryEntity().visual.gameObject.GetComponent<DummyUI>().GetCoinText().text = value.ToString();
    }

    private GameEntity GetRepositoryEntity() => _contexts.game.GetGroup(GameMatcher.Coin).GetEntities().First();

    private bool IsAtEndOfDrinkLevel(int currentDrinkCoinLevel) =>
        currentDrinkCoinLevel == _drinkCoinLevelsPrice.coinLevels.Length - 1;

    private bool IsEligibleToUpgradeDrink(int coinAmount, int currentDrinkCoinLevel) =>
       coinAmount >= _drinkCoinLevelsPrice.coinLevels[currentDrinkCoinLevel + 1].upgradeCost;

    private void ShowUpgradeInfo(int currentDrinkCoinLevel)
    {
        _dummyUI.GetUpgradeDrinkCostText().text = _drinkCoinLevelsPrice.coinLevels[currentDrinkCoinLevel + 1].upgradeCost.ToString();
        _dummyUI.GetDrinkPriceText().text = _drinkCoinLevelsPrice.coinLevels[currentDrinkCoinLevel + 1].coin.ToString();
        _dummyUI.GetUpgradeInfo().SetActive(true);
    }

    private void OnUpgrade()
    {
        HideUpgradeInfo();
        _onClickUpgrade?.OnNext(Unit.Default);
    }

    private void HideUpgradeInfo()
    {
        _dummyUI.GetUpgradeInfo().SetActive(false);
    }
}
