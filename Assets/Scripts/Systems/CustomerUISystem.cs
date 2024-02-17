using Entitas;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public sealed class CustomerUISystem : ReactiveSystem<GameEntity>
{
    public CustomerUISystem(Contexts contexts) : base(contexts.game)
    {
    }

    protected override void Execute(List<GameEntity> entities)
    {
        DisplayCustomerPopopFor(entities.Where(x => x.isShowCanvas));
        HideCuctomerPopupFor(entities.Where(x => !x.isShowCanvas));
    }

    protected override bool Filter(GameEntity entity) => entity.isCustomer;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.Customer, GameMatcher.ShowCanvas).AddedOrRemoved());

    private static void DisplayCustomerPopopFor(IEnumerable<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            DisplayCanvasFor(entity);
            UpdateEntityText(entity);
        }
    }

    private static void HideCuctomerPopupFor(IEnumerable<GameEntity> entities)
    {
        foreach (var entity in entities)
            HideCanvasFor(entity);
    }

    private static void DisplayCanvasFor(GameEntity entity)
    {
        GetEntityCanvasObj(entity).SetActive(true);
    }

    private static void HideCanvasFor(GameEntity entity)
    {
        GetEntityCanvasObj(entity).SetActive(false);
    }

    private static void UpdateEntityText(GameEntity entity)
    {
        GetEntityTextObj(entity).text = entity.quantity.value.ToString();
    }


    private static GameObject GetEntityCanvasObj(GameEntity entity) =>
        entity.visual.gameObject.transform.GetChild(0).gameObject;

    private static TMP_Text GetEntityTextObj(GameEntity entity) =>
        entity.visual.gameObject.GetComponentInChildren<TMP_Text>();
}
