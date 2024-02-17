using DG.Tweening;
using Entitas;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public sealed class ChefUISystem : ReactiveSystem<GameEntity>, IInitializeSystem
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private IGroup<GameEntity> _chefCooldownGroup;

    public ChefUISystem(Contexts contexts) : base(contexts.game)
    {
        _chefCooldownGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Chef, GameMatcher.Cooldown));
    }

    ~ChefUISystem()
    {
        _compositeDisposable.Dispose();
    }

    public void Initialize()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        DummyUISystem.OnClickRestaurantUpgrade
                    .Subscribe(_ =>
                    {
                        HideCooldownUIFor(_chefCooldownGroup.GetEntities());
                    }).AddTo(_compositeDisposable);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        ShowCooldownUIFor(entities.Where(x => x.hasCooldown));
        HideCooldownUIFor(entities.Where(x => !x.hasCooldown));
    }

    protected override bool Filter(GameEntity entity) => entity.isChef;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.Chef, GameMatcher.Cooldown).AddedOrRemoved());

    private static void ShowCooldownUIFor(IEnumerable<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            DisplayCanvas(entity);
            FillTheScrollbar(entity);
        }
    }

    private static void DisplayCanvas(GameEntity entity)
    {
        GetUICanvas(entity).SetActive(true);
    }

    private static Scrollbar GetScrollbarOf(GameEntity entity) =>
       GetUICanvas(entity).GetComponentInChildren<Scrollbar>();

    private static void FillTheScrollbar(GameEntity entity)
    {
        DOTween.To(() => 0f, x => GetScrollbarOf(entity).size = x, 1f, entity.cooldown.duration);
    }

    private static void HideCooldownUIFor(IEnumerable<GameEntity> entities)
    {
        foreach (var entity in entities)
            HideCanvas(entity);
    }

    private static void HideCanvas(GameEntity entity)
    {
        GetUICanvas(entity).SetActive(false);
    }

    private static GameObject GetUICanvas(GameEntity entity) =>
        entity.visual.gameObject.transform.GetChild(0).gameObject;
}
