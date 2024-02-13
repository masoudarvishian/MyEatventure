//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public WaitingCustomerComponent waitingCustomer { get { return (WaitingCustomerComponent)GetComponent(GameComponentsLookup.WaitingCustomer); } }
    public bool hasWaitingCustomer { get { return HasComponent(GameComponentsLookup.WaitingCustomer); } }

    public void AddWaitingCustomer(UnityEngine.Vector3 newPosition) {
        var index = GameComponentsLookup.WaitingCustomer;
        var component = (WaitingCustomerComponent)CreateComponent(index, typeof(WaitingCustomerComponent));
        component.position = newPosition;
        AddComponent(index, component);
    }

    public void ReplaceWaitingCustomer(UnityEngine.Vector3 newPosition) {
        var index = GameComponentsLookup.WaitingCustomer;
        var component = (WaitingCustomerComponent)CreateComponent(index, typeof(WaitingCustomerComponent));
        component.position = newPosition;
        ReplaceComponent(index, component);
    }

    public void RemoveWaitingCustomer() {
        RemoveComponent(GameComponentsLookup.WaitingCustomer);
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

    static Entitas.IMatcher<GameEntity> _matcherWaitingCustomer;

    public static Entitas.IMatcher<GameEntity> WaitingCustomer {
        get {
            if (_matcherWaitingCustomer == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.WaitingCustomer);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherWaitingCustomer = matcher;
            }

            return _matcherWaitingCustomer;
        }
    }
}
