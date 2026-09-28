using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.AI;

public class AIUnitTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void AIUnitTestScriptSimplePasses()
    {
        
        
        // Use the Assert class to test conditions
    }

    

    [SetUp]
    public void Setup()
    {
        GameObject thing = new GameObject()l
            thing.AddComponent<>
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator AIUnitTestScriptWithEnumeratorPasses()
    {
        DreamerThrowing throwing -new DreamerThrowing();
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
