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
        private IModule module;
        public virtual void Init()
        {
            if (!hasInit)
            {
                OnInit();
                hasInit = true;
            } 
        }

        public void SetModule(IModule module)
        {
            this.module = module;
        }
        protected abstract void OnInit();
    }

}