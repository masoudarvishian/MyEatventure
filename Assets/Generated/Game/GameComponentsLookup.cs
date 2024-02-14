//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentLookupGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public static class GameComponentsLookup {

    public const int Chef = 0;
    public const int ChefVisual = 1;
    public const int Cooldown = 2;
    public const int CustomerIndex = 3;
    public const int Delivered = 4;
    public const int Mover = 5;
    public const int Position = 6;
    public const int PreparingOrder = 7;
    public const int Quantity = 8;
    public const int TargetPosition = 9;
    public const int WaitingCustomer = 10;

    public const int TotalComponents = 11;

    public static readonly string[] componentNames = {
        "Chef",
        "ChefVisual",
        "Cooldown",
        "CustomerIndex",
        "Delivered",
        "Mover",
        "Position",
        "PreparingOrder",
        "Quantity",
        "TargetPosition",
        "WaitingCustomer"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(ChefComponent),
        typeof(ChefVisualComponent),
        typeof(CooldownComponent),
        typeof(CustomerIndexComponent),
        typeof(DeliveredComponent),
        typeof(MoverComponent),
        typeof(PositionComponent),
        typeof(PreparingOrder),
        typeof(QuantityComponent),
        typeof(TargetPositionComponent),
        typeof(WaitingCustomerComponent)
    };
}
