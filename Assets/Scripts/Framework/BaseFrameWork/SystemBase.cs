using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfStateFrame
{
    public interface ISystem
    {
        public void Init();
    }

    public abstract class SystemBase : ISystem
    { 
        private bool hasInit; 
        public virtual void Init()
        {
            if (!hasInit)
            {
                OnInit();
                hasInit = true;
            } 
        }
         
        protected abstract void OnInit();

        protected IState Current {
            get { return GameMainLoop.Instance.CurrentState; } 
    }
}

}