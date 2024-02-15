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
        foreach(var entity in entities.Where(x => x.isShowCanvas))
        {
            entity.visual.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            entity.visual.gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = entity.quantity.value.ToString();
        }

        foreach (var entity in entities.Where(x => !x.isShowCanvas))
        {
            entity.visual.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isCustomer;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.ShowCanvas).AddedOrRemoved());
    }
}
