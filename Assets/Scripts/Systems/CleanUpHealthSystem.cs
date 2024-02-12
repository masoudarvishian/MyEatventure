using Entitas;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace Assets.Scripts.Systems
{
    public sealed class CleanUpHealthSystem : ICleanupSystem
    {
        private Contexts contexts;
        private IGroup<GameEntity> entitiesGroup;

        public CleanUpHealthSystem(Contexts contexts)
        {
            this.contexts = contexts;
            entitiesGroup = this.contexts.game.GetGroup(GameMatcher.Health);
        }

        public void Cleanup()
        {
            foreach (var item in entitiesGroup.GetEntities())
                item.Destroy();
        }
    }
}
