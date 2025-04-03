using SabaSimpleDIContainer;
using SabaSimpleDIContainer.Unity;
using UnityEngine;

public class TestLifeTimeScope:LifeTimeScope
{
    [SerializeField]
    private TestClassA testClassA;
    protected override void Configure(IContainer container)
    {
        if(testClassA!=null)
        {
            container.RegisterComponent(testClassA);
        }
        container.RegisterEntryPoint<TestClassB>();
        TestClassC testClassC = new TestClassC();
        container.Register<TestClassC>(testClassC,LifeTime.singleton);
        container.Register<ITest>(testClassA,LifeTime.Multiple);
        container.Register<TestClassD>(LifeTime.singleton);
    }

}