using Entitas;
using Entitas.Unity;
using System.Linq;
using TMPro;
using UniRx;

public sealed class RepositorySystem : IInitializeSystem
{
    private readonly Contexts _contexts;
    private readonly TMP_Text _coinText;

    private const int InitialCoinValue = 10;
    private int coinsPerPurchase = 10;

    private CompositeDisposable _compositeDisposable = new();

    public RepositorySystem(Contexts contexts, TMPro.TMP_Text coinText)
    {
        _contexts = contexts;
        _coinText = coinText;
    }

    ~RepositorySystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        DeliveryOrderSystem.OnOrderIsDelivered.Subscribe(_ =>
        {
            var repositoryEntity = _contexts.game.GetGroup(GameMatcher.Coin).GetEntities().First();
            var currentCoins = repositoryEntity.coin.value;
            var newValue = currentCoins + coinsPerPurchase;
            repositoryEntity.ReplaceCoin(newValue);
            repositoryEntity.visual.gameObject.GetComponent<TMP_Text>().text = newValue.ToString();
        }).AddTo(_compositeDisposable);

        CreateAndLinkEntity();
    }

    private void CreateAndLinkEntity()
    {
        var e = _contexts.game.CreateEntity();
        e.AddCoin(InitialCoinValue);

        _coinText.text = InitialCoinValue.ToString();

        e.AddVisual(_coinText.gameObject);
        _coinText.gameObject.Link(e);
    }
}
