using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.RegistrationByConvention;

namespace PlanSample
{
    public class PlanSample
    {
        private IUnityContainer container;

        public PlanSample()
        {

        }

        public void InitializeComponent()
        {
            container = new UnityContainer();
            container.RegisterTypes(
                AllClasses.FromAssembliesInBasePath(),
                WithMappings.FromMatchingInterface,
                WithName.Default
            );
        }

        public void Run()
        {
            ITest test = container.Resolve<ITest>();
            test.Do();
        }
    }

    public interface ITest
    {
        void Do();
    }

    public class Test : ITest
    {
        public void Do()
        {
            Console.Write("Test");
        }
    }
}
