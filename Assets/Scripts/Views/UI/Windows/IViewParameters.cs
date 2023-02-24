namespace Views
{
    public interface IViewParameters<T> where T:class, IViewData
    {
        void SetData(T data);
        T Data { get; }
    }
    public interface IViewData
    {
    }
}