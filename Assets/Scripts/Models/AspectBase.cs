namespace Models
{
    public abstract class AspectBase<T,T1> where T : new()
    {
        public abstract T FromSaveData(T1 saveData);
        public abstract T1 ToSaveData(T saveData);

    }
}