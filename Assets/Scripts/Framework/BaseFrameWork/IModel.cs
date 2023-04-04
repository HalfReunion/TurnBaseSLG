namespace HalfStateFrame
{
    public interface IMessageHandler
    {
        void Init(params IModel[] val);
    }

    public abstract class MessageHandlerBase : IMessageHandler
    {
        private IModel[] val;
        public IModel[] GetValue => val;
        public void Init(params IModel[] val) {
            this.val = val;
        }

        
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