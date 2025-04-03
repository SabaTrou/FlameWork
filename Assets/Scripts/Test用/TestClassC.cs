using SabaSimpleDIContainer;
public class TestClassC:ITest
{
    [Injection]
    private TestClassD testClassD;
    public TestClassD TestClassD => testClassD;

    public string Name => throw new System.NotImplementedException();

    public int TestValue { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Test()
    {
        throw new System.NotImplementedException();
    }
}