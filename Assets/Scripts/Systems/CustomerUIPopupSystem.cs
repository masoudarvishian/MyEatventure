using Entitas;
using System.Collections.Generic;
using System.Linq;

public sealed class CustomerUIPopupSystem : ReactiveSystem<GameEntity>
{
    public CustomerUIPopupSystem(Contexts contexts) : base(contexts.game)
    {
    }

    protected override void Execute(List<GameEntity> entities)
    {
        DisplayCustomerPopopFor(entities.Where(x => x.isShowCanvas));
        HideCuctomerPopupFor(entities.Where(x => !x.isShowCanvas));
    }

    private static void DisplayCustomerPopopFor(IEnumerable<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.visual.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            entity.visual.gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = entity.quantity.value.ToString();
        }
    }

    private static void HideCuctomerPopupFor(IEnumerable<GameEntity> entities)
    {
        foreach (var entity in entities)
            entity.visual.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    protected override bool Filter(GameEntity entity) => entity.isCustomer;

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
        context.CreateCollector(GameMatcher.AllOf(GameMatcher.ShowCanvas).AddedOrRemoved());
}
