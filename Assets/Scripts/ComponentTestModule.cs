using System.Diagnostics;
using ComponentSystem;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
public class ComponentTestModule : MonoBehaviour
{
    private Stopwatch _stopwatch = new Stopwatch();
    private int _testCount = 1000000;
    private void Start()
    {
        TestMonoBehaviour();
        TestComponentSystem();
    }
    private void TestComponentSystem()
    {
        TestComponentHolder testComponentHolder = new TestComponentHolder(20);
        AddComponentAll(testComponentHolder);

        _stopwatch.Start();
        for (int i = 0; i < _testCount; i++)
        {
            int index = Random.Range(0, 20);
            GetComponentByIndex(index, testComponentHolder);
        }
        _stopwatch.Stop();
        Debug.Log("ComponentSystem:Time taken to get 1,000,000 components: " + _stopwatch.ElapsedMilliseconds + "ms");
        _stopwatch.Reset();
    }
    private void TestMonoBehaviour()
    {
        GameObject testGameObject = new GameObject();
        AddComponentAll(testGameObject);
        _stopwatch.Start();
        for (int i = 0; i < _testCount; i++)
        {
            int index = Random.Range(0, 20);
            GetComponentByIndex(index, testGameObject);
        }
        _stopwatch.Stop();
        Debug.Log("MonoBehaviour:Time taken to get 1,000,000 components: " + _stopwatch.ElapsedMilliseconds + "ms");
        _stopwatch.Reset();
    }

    private void AddComponentAll(GameObject game)
    {
        game.AddComponent<TestMonoComponent>();
        game.AddComponent<TestMonoComponent1>();
        game.AddComponent<TestMonoComponent2>();
        game.AddComponent<TestMonoComponent3>();
        game.AddComponent<TestMonoComponent4>();
        game.AddComponent<TestMonoComponent5>();
        game.AddComponent<TestMonoComponent6>();
        game.AddComponent<TestMonoComponent7>();
        game.AddComponent<TestMonoComponent8>();
        game.AddComponent<TestMonoComponent9>();
        game.AddComponent<TestMonoComponent10>();
        game.AddComponent<TestMonoComponent11>();
        game.AddComponent<TestMonoComponent12>();
        game.AddComponent<TestMonoComponent13>();
        game.AddComponent<TestMonoComponent14>();
        game.AddComponent<TestMonoComponent15>();
        game.AddComponent<TestMonoComponent16>();
        game.AddComponent<TestMonoComponent17>();
        game.AddComponent<TestMonoComponent18>();
        game.AddComponent<TestMonoComponent19>();
    }
    private void AddComponentAll(ComponentHolder holder)
    {
        holder.AddComponent(new TestMonoComponent());
        holder.AddComponent(new TestMonoComponent1());
        holder.AddComponent(new TestMonoComponent2());
        holder.AddComponent(new TestMonoComponent3());
        holder.AddComponent(new TestMonoComponent4());
        holder.AddComponent(new TestMonoComponent5());
        holder.AddComponent(new TestMonoComponent6());
        holder.AddComponent(new TestMonoComponent7());
        holder.AddComponent(new TestMonoComponent8());
        holder.AddComponent(new TestMonoComponent9());
        holder.AddComponent(new TestMonoComponent10());
        holder.AddComponent(new TestMonoComponent11());
        holder.AddComponent(new TestMonoComponent12());
        holder.AddComponent(new TestMonoComponent13());
        holder.AddComponent(new TestMonoComponent14());
        holder.AddComponent(new TestMonoComponent15());
        holder.AddComponent(new TestMonoComponent16());
        holder.AddComponent(new TestMonoComponent17());
        holder.AddComponent(new TestMonoComponent18());
        holder.AddComponent(new TestMonoComponent19());
    }
    private void GetComponentByIndex(int index, GameObject game)
    {
        switch (index)
        {
            case 0: game.gameObject.GetComponent<TestMonoComponent>(); break;
            case 1: game.gameObject.GetComponent<TestMonoComponent1>(); break;
            case 2: game.gameObject.GetComponent<TestMonoComponent2>(); break;
            case 3: game.gameObject.GetComponent<TestMonoComponent3>(); break;
            case 4: game.gameObject.GetComponent<TestMonoComponent4>(); break;
            case 5: game.gameObject.GetComponent<TestMonoComponent5>(); break;
            case 6: game.gameObject.GetComponent<TestMonoComponent6>(); break;
            case 7: game.gameObject.GetComponent<TestMonoComponent7>(); break;
            case 8: game.gameObject.GetComponent<TestMonoComponent8>(); break;
            case 9: game.gameObject.GetComponent<TestMonoComponent9>(); break;
            case 10: game.gameObject.GetComponent<TestMonoComponent10>(); break;
            case 11: game.gameObject.GetComponent<TestMonoComponent11>(); break;
            case 12: game.gameObject.GetComponent<TestMonoComponent12>(); break;
            case 13: game.gameObject.GetComponent<TestMonoComponent13>(); break;
            case 14: game.gameObject.GetComponent<TestMonoComponent14>(); break;
            case 15: game.gameObject.GetComponent<TestMonoComponent15>(); break;
            case 16: game.gameObject.GetComponent<TestMonoComponent16>(); break;
            case 17: game.gameObject.GetComponent<TestMonoComponent17>(); break;
            case 18: game.gameObject.GetComponent<TestMonoComponent18>(); break;
            case 19: game.gameObject.GetComponent<TestMonoComponent19>(); break;
        }
    }
    private void GetComponentByIndex(int index, ComponentHolder holder)
    {
        switch (index)
        {
            case 0: holder.GetComponent<TestMonoComponent>(); break;
            case 1: holder.GetComponent<TestMonoComponent1>(); break;
            case 2: holder.GetComponent<TestMonoComponent2>(); break;
            case 3: holder.GetComponent<TestMonoComponent3>(); break;
            case 4: holder.GetComponent<TestMonoComponent4>(); break;
            case 5: holder.GetComponent<TestMonoComponent5>(); break;
            case 6: holder.GetComponent<TestMonoComponent6>(); break;
            case 7: holder.GetComponent<TestMonoComponent7>(); break;
            case 8: holder.GetComponent<TestMonoComponent8>(); break;
            case 9: holder.GetComponent<TestMonoComponent9>(); break;
            case 10: holder.GetComponent<TestMonoComponent10>(); break;
            case 11: holder.GetComponent<TestMonoComponent11>(); break;
            case 12: holder.GetComponent<TestMonoComponent12>(); break;
            case 13: holder.GetComponent<TestMonoComponent13>(); break;
            case 14: holder.GetComponent<TestMonoComponent14>(); break;
            case 15: holder.GetComponent<TestMonoComponent15>(); break;
            case 16: holder.GetComponent<TestMonoComponent16>(); break;
            case 17: holder.GetComponent<TestMonoComponent17>(); break;
            case 18: holder.GetComponent<TestMonoComponent18>(); break;
            case 19: holder.GetComponent<TestMonoComponent19>(); break;
        }
    }
}
    public class TestMonoComponent : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent1 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent2 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent3 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent4 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent5 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent6 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent7 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent8 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent9 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent10 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent11 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent12 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent13 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent14 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent15 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent16 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent17 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent18 : MonoBehaviour, IComponent
    {
    }
    public class TestMonoComponent19 : MonoBehaviour, IComponent
    {
    }
    public class TestComponent : IComponent
    {
    }
    public class TestComponent1 : IComponent
    {
    }
    public class TestComponent2 : IComponent
    {
    }
#endif
