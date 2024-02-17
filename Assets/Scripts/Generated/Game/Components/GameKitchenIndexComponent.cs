//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public KitchenIndex kitchenIndex { get { return (KitchenIndex)GetComponent(GameComponentsLookup.KitchenIndex); } }
    public bool hasKitchenIndex { get { return HasComponent(GameComponentsLookup.KitchenIndex); } }

    public void AddKitchenIndex(int newValue) {
        var index = GameComponentsLookup.KitchenIndex;
        var component = (KitchenIndex)CreateComponent(index, typeof(KitchenIndex));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceKitchenIndex(int newValue) {
        var index = GameComponentsLookup.KitchenIndex;
        var component = (KitchenIndex)CreateComponent(index, typeof(KitchenIndex));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveKitchenIndex() {
        RemoveComponent(GameComponentsLookup.KitchenIndex);
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

    static Entitas.IMatcher<GameEntity> _matcherKitchenIndex;

    public static Entitas.IMatcher<GameEntity> KitchenIndex {
        get {
            if (_matcherKitchenIndex == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.KitchenIndex);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherKitchenIndex = matcher;
            }

            return _matcherKitchenIndex;
        }
    }
}
