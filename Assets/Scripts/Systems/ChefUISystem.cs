using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public sealed class ChefUISystem : ReactiveSystem<GameEntity>
{
    public ChefUISystem(Contexts contexts) : base(contexts.game)
    {
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities.Where(x => x.hasCooldown))
        {
            DisplayCanvas(entity);
        }

        foreach (var entity in entities.Where(x => !x.hasCooldown))
        {
            HideCanvas(entity);
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isChef;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Chef, GameMatcher.Cooldown).AddedOrRemoved());
    }

    private static void HideCanvas(GameEntity entity)
    {
        GetUICanvas(entity).SetActive(false);
    }

    private static void DisplayCanvas(GameEntity entity)
    {
        GetUICanvas(entity).SetActive(true);
    }

    private static GameObject GetUICanvas(GameEntity entity) =>
        entity.visual.gameObject.transform.GetChild(0).gameObject;
}
