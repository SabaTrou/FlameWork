using SabaSimpleDIContainer;
using SabaSimpleDIContainer.Unity;
using UnityEngine;

public class TestClassB:IStartable
{
    [Injection]
    private TestClassA testClassC;
    [Injection]
    private ITest test;
    public TestClassA TestClassC => testClassC;
    void IStartable.Start()
    {
        Debug.Log(testClassC.GetHashCode()==test.GetHashCode());
    }
}