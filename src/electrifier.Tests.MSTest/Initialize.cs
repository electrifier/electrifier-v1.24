using Microsoft.Windows.ApplicationModel.DynamicDependency;

[assembly: WinUITestTarget(typeof(electrifier.App))]

namespace electrifier.Tests.MSTest;

[TestClass]
public class Initialize
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
        // TODO: Initialize the appropriate version of the Windows App SDK.
        // This is required when testing MSIX apps that are framework-dependent on the Windows App SDK.
        //Assert.IsTrue(Bootstrap.TryInitialize(0x00010001, out var _));
    }

    [TestMethod]
    public void TestMethod()
    {
        Assert.IsTrue(true);
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        Bootstrap.Shutdown();
    }
}
