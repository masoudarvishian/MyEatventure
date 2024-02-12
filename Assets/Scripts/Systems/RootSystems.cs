using Assets.Scripts.Systems;

public class RootSystems : Feature
{
    public RootSystems(Contexts contexts)
    {
        Add(new LogHealthSystem(contexts));
        Add(new CreatePlayerSystem(contexts));
        //Add(new CleanUpHealthSystem(contexts));
    }
}
