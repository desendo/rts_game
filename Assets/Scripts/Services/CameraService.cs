using Locator;


namespace Services
{
    public interface ICameraService
    {
    }


    public class CameraService : ICameraService, ITick
    {
        public void Tick(float dt)
        {

        }
    }
}