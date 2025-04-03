using SabaSimpleDIContainer;
using UnityEngine;
using System.Diagnostics;
using Debug= UnityEngine.Debug;
using System.Collections.Generic;
using SabaSimpleDIContainer.Unity;

public class TestClassA:MonoBehaviour,ITest
{
    [Injection]
    IResolver resolver;
    private Stopwatch stopwatch = new Stopwatch();

    public string Name => throw new System.NotImplementedException();

    public int TestValue { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Test()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
       
    }

}