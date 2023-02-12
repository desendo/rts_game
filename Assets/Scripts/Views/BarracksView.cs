using Models;
using UniRx;
namespace Views
{
    public class BarracksView : UnitView
    {
        public override void Bind(IModel model)
        {
            base.Bind(model);
            if (model is UnitModel unitModelBase)
            {
                foreach (var aspect in unitModelBase.Aspects)
                {
                    if (aspect is AspectTransform aspectTransform)
                    {
                        aspectTransform.Position.Subscribe(x => transform.position = x).AddTo(_sup);
                        aspectTransform.Rotation.Subscribe(x => transform.rotation = x).AddTo(_sup);
                    }
                }
            }

        }
    }
}