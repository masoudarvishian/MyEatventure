//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public TargetDeskPositionComponent targetDeskPosition { get { return (TargetDeskPositionComponent)GetComponent(GameComponentsLookup.TargetDeskPosition); } }
    public bool hasTargetDeskPosition { get { return HasComponent(GameComponentsLookup.TargetDeskPosition); } }

    public void AddTargetDeskPosition(UnityEngine.Vector3 newValue) {
        var index = GameComponentsLookup.TargetDeskPosition;
        var component = (TargetDeskPositionComponent)CreateComponent(index, typeof(TargetDeskPositionComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceTargetDeskPosition(UnityEngine.Vector3 newValue) {
        var index = GameComponentsLookup.TargetDeskPosition;
        var component = (TargetDeskPositionComponent)CreateComponent(index, typeof(TargetDeskPositionComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveTargetDeskPosition() {
        RemoveComponent(GameComponentsLookup.TargetDeskPosition);
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

    static Entitas.IMatcher<GameEntity> _matcherTargetDeskPosition;

    public static Entitas.IMatcher<GameEntity> TargetDeskPosition {
        get {
            if (_matcherTargetDeskPosition == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TargetDeskPosition);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherTargetDeskPosition = matcher;
            }

            return _matcherTargetDeskPosition;
        }
    }
}