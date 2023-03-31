namespace HalfStateFrame
{
    public interface IMessageHandler
    {
        void Init();
    }

    public abstract class MessageHandlerBase : IMessageHandler
    {
        public abstract void Init();
    }

    public interface IModel
    {
        public void Init();
    }

    public abstract class Model<T> : IModel
    {
        private T t;

        public T GetValue()
        {
            return t;
        }

        public virtual void Init()
        {
        }
    }
}