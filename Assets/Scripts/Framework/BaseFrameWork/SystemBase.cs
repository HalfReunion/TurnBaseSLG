using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfStateFrame
{
    public interface ISystem
    {
        /// <summary>
        /// 组件初始化，或者数据初始化
        /// </summary>
        public void Init();

        public void RenderInit();

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
        /// <summary>
        /// 渲染初始化
        /// </summary>
        public virtual void RenderInit() { }
}

}