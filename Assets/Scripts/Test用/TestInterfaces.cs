public interface ITest
{
    string Name { get; }
    int TestValue { get; set; }
    void Test();
}
public interface ITest2
{
    int TestValue { get; set; }
    void Test2();
}