using System.Collections.Generic;
using Data;
using UniRx;

namespace Models.Aspects
{
    public class AspectQueue
    {
        public readonly List<string> List = new List<string>();
        public readonly Subject<Unit> OnChange = new Subject<Unit>();

        public AspectQueue()
        {
        }
        public AspectQueue(SaveDataProductionQueue saveData)
        {
            List = new List<string>(saveData.List);
            OnChange.OnNext(Unit.Default);
        }
    }

}
