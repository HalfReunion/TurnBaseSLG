using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HalfStateFrame
{
    public interface IEventItem
    {
       
    }

    public  class EventItem<T> : IEventItem
    {
        Action<T> action ;

        public void RegisterEvent(Action<T> onAction)
        {
            action += onAction;
        }

        public void UnRegisterEvent(Action<T> onAction)
        {
            action -= onAction;
        }

        public void ClearEvent()
        {
            action = null;
        }

        public void Trigger(T param)
        {
            action?.Invoke(param);
        }
    }

}