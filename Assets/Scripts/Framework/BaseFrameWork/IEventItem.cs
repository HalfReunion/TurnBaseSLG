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

    public class EventItem<T1,T2> : IEventItem
    {
        Action<T1,T2> action;

        public void RegisterEvent(Action<T1, T2> onAction)
        {
            action += onAction;
        }

        public void UnRegisterEvent(Action<T1, T2> onAction)
        {
            action -= onAction;
        }

        public void ClearEvent()
        {
            action = null;
        }

        public void Trigger(T1 p,T2 p2)
        {
            action?.Invoke(p,p2);
        }
    }

}