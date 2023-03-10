using Models.Components;

namespace Views
{
    public class BuilderView : VehicleView
    {
        public override void Bind(int entity)
        {
            base.Bind(entity);
            _world.GetPool<ComponentBuilder>().Add(entity);
        }
    }
}