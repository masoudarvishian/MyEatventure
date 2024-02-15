using Entitas;
using Entitas.Unity;
using System.Linq;
using UniRx;

public sealed class RepositorySystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly DummyUI _dummyUI;
    private readonly DrinkCoinLevelsPriceSO _coinLevelsPrice;
    private const int InitialCoinValue = 10;

    private CompositeDisposable _compositeDisposable = new();

    private int _currentDrinkCoinLevel = 0;

    public RepositorySystem(
        Contexts contexts,
        DummyUI dummyUI,
        DrinkCoinLevelsPriceSO drinkCoinLevelsPrice)
    {
        _contexts = contexts;
        _dummyUI = dummyUI;
        _coinLevelsPrice = drinkCoinLevelsPrice;
    }

    ~RepositorySystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        DeliveryOrderSystem.OnOrderIsDelivered.Subscribe(_ => OnOrderIsDelivered()).AddTo(_compositeDisposable);
        _dummyUI.GetUpgradeBtn().OnClickAsObservable().Subscribe(_ =>
        {
            UpdateEntityWithNewValue(_coinLevelsPrice.coinLevels[_currentDrinkCoinLevel].upgradePrice * -1);
            _currentDrinkCoinLevel++;
            _dummyUI.GetUpgradeInfo().SetActive(false);
        }).AddTo(_compositeDisposable);
        CreateAndLinkEntity();
    }

    private void OnOrderIsDelivered()
    {
        int coinAmount = UpdateEntityWithNewValue(_coinLevelsPrice.coinLevels[_currentDrinkCoinLevel].coin);

        // handle upgrade UI
        if (_currentDrinkCoinLevel == _coinLevelsPrice.coinLevels.Length - 1)
            return;

        if (coinAmount >= _coinLevelsPrice.coinLevels[_currentDrinkCoinLevel + 1].upgradePrice)
        {
            _dummyUI.GetUpgradeDrinkCostText().text = _coinLevelsPrice.coinLevels[_currentDrinkCoinLevel + 1].upgradePrice.ToString();
            _dummyUI.GetDrinkPriceText().text = _coinLevelsPrice.coinLevels[_currentDrinkCoinLevel + 1].coin.ToString();
            _dummyUI.GetUpgradeInfo().SetActive(true);
        }
        else
        {
            _dummyUI.GetUpgradeInfo().SetActive(false);
        }
    }

    private int UpdateEntityWithNewValue(int newValue)
    {
        var repositoryEntity = GetRepositoryEntity();
        var currentCoins = repositoryEntity.coin.value;
        var result = currentCoins + newValue;
        repositoryEntity.ReplaceCoin(result);
        repositoryEntity.visual.gameObject.GetComponent<DummyUI>().GetCoinText().text = result.ToString();
        return result;
    }

    private void CreateAndLinkEntity()
    {
        var e = _contexts.game.CreateEntity();
        e.AddCoin(InitialCoinValue);
        e.AddVisual(_dummyUI.gameObject);
        _dummyUI.GetCoinText().text = InitialCoinValue.ToString();
        _dummyUI.gameObject.Link(e);
    }

    private GameEntity GetRepositoryEntity() => _contexts.game.GetGroup(GameMatcher.Coin).GetEntities().First();
}
