//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    static readonly BuysKitchenComponent buysKitchenComponent = new BuysKitchenComponent();

    public bool isBuysKitchen {
        get { return HasComponent(GameComponentsLookup.BuysKitchen); }
        set {
            if (value != isBuysKitchen) {
                var index = GameComponentsLookup.BuysKitchen;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : buysKitchenComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherBuysKitchen;

    public static Entitas.IMatcher<GameEntity> BuysKitchen {
        get {
            if (_matcherBuysKitchen == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.BuysKitchen);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherBuysKitchen = matcher;
            }

            return _matcherBuysKitchen;
        }
    }
}
