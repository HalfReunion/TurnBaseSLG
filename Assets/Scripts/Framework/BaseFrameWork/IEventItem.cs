using System;

namespace HalfStateFrame
{
    public interface IEventItem
    {
    }

    public class EventItem<T> : IEventItem
    {
        private Action<T> action;

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

    public class EventItem<T1, T2> : IEventItem
    {
        private Action<T1, T2> action;

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

        public void Trigger(T1 p, T2 p2)
        {
            action?.Invoke(p, p2);
        }
    }

    public class EventItem : IEventItem
    {
        private Action action;

        public void RegisterEvent(Action onAction)
        {
            action += onAction;
        }

        public void UnRegisterEvent(Action onAction)
        {
            action -= onAction;
        }

        public void ClearEvent()
        {
            action = null;
        }

        public void Trigger()
        {
            action?.Invoke();
        }
    }
}