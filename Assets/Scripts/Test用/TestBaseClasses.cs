
using UnityEngine;

public abstract class BaseTest:MonoBehaviour
{
    public abstract void Test();
    private void Hoge()
    {
        GetComponent<BaseTest2>().Test();
    }
}
public abstract class BaseTest2
{
    public abstract void Test();
}