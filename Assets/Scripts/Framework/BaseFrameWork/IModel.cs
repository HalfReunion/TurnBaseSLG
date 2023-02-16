using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

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

    public class Model<T> : IModel  where T : MessageHandlerBase
    { 
        private T t;

        public T GetValue() {
            return t;
        }

        public virtual void Init() {  
        }

        public Model(T t)
        {
            this.t = t; 
        }

        public static Model<T> GetModel(T t) {
            Model<T> model = new Model<T>(t);
            return model;
        }

       
    }
}