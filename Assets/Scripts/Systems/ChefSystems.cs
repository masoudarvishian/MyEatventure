public class ChefSystems : Feature
{
    public ChefSystems(Contexts contexts)
    {
        Add(new CreateChefSystem(contexts));
    }
}
