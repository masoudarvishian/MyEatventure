using Entitas;
using Entitas.Unity;
using System.Linq;
using TMPro;
using UniRx;

public sealed class RepositorySystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly TMP_Text _coinText;
    private readonly CoinLevelsPriceSO _coinLevelsPrice;
    private const int InitialCoinValue = 10;
    private int coinsPerPurchase = 10;

    private CompositeDisposable _compositeDisposable = new();

    public RepositorySystem(Contexts contexts, TMP_Text coinText, CoinLevelsPriceSO coinLevelsPrice)
    {
        _contexts = contexts;
        _coinText = coinText;
        _coinLevelsPrice = coinLevelsPrice;
    }

    ~RepositorySystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        DeliveryOrderSystem.OnOrderIsDelivered.Subscribe(_ => OnOrderIsDelivered()).AddTo(_compositeDisposable);
        CreateAndLinkEntity();
    }

    private void OnOrderIsDelivered()
    {
        var repositoryEntity = GetRepositoryEntity();
        var currentCoins = repositoryEntity.coin.value;
        var newValue = currentCoins + coinsPerPurchase;
        repositoryEntity.ReplaceCoin(newValue);
        repositoryEntity.visual.gameObject.GetComponent<TMP_Text>().text = newValue.ToString();
    }

    private void CreateAndLinkEntity()
    {
        var e = _contexts.game.CreateEntity();
        e.AddCoin(InitialCoinValue);
        e.AddVisual(_coinText.gameObject);
        _coinText.text = InitialCoinValue.ToString();
        _coinText.gameObject.Link(e);
    }

    private GameEntity GetRepositoryEntity() => _contexts.game.GetGroup(GameMatcher.Coin).GetEntities().First();
}
