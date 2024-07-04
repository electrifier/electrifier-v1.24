using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Tests.MSTest;

[TestClass]
public class SimpleUnitTest
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        Debug.WriteLine("ClassInitialize");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Debug.WriteLine("ClassCleanup");
    }

    [TestInitialize]
    public void TestInitialize()
    {
        Debug.WriteLine("TestInitialize");
    }

    [TestCleanup]
    public void TestCleanup()
    {
        Debug.WriteLine("TestCleanup");
    }

    [TestMethod]
    public void TestMethod()
    {
        Assert.IsTrue(true);
    }

    [UITestMethod]
    public void UITestMethod()
    {
        Assert.AreEqual(0, new Grid().ActualWidth);
    }

    [TestMethod]
    public void TestStringWriter()
    {
        //using var sw = new StringWriter();
        //Console.SetOut(sw);
        ////HelloWorld.Program.Main();

        //var result = sw.ToString().Trim();
        ////Assert.AreEqual(Expected, result);
    }
}
