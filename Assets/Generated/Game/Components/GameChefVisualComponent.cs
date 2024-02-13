//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public ChefVisualComponent chefVisual { get { return (ChefVisualComponent)GetComponent(GameComponentsLookup.ChefVisual); } }
    public bool hasChefVisual { get { return HasComponent(GameComponentsLookup.ChefVisual); } }

    public void AddChefVisual(UnityEngine.GameObject newGameObject) {
        var index = GameComponentsLookup.ChefVisual;
        var component = (ChefVisualComponent)CreateComponent(index, typeof(ChefVisualComponent));
        component.gameObject = newGameObject;
        AddComponent(index, component);
    }

    public void ReplaceChefVisual(UnityEngine.GameObject newGameObject) {
        var index = GameComponentsLookup.ChefVisual;
        var component = (ChefVisualComponent)CreateComponent(index, typeof(ChefVisualComponent));
        component.gameObject = newGameObject;
        ReplaceComponent(index, component);
    }

    public void RemoveChefVisual() {
        RemoveComponent(GameComponentsLookup.ChefVisual);
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

    static Entitas.IMatcher<GameEntity> _matcherChefVisual;

    public static Entitas.IMatcher<GameEntity> ChefVisual {
        get {
            if (_matcherChefVisual == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.ChefVisual);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherChefVisual = matcher;
            }

            return _matcherChefVisual;
        }
    }
}