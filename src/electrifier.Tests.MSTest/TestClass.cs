﻿using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Tests.MSTest;

/// <summary>
/// Summary description for TestClass
/// <br/><see href="https://docs.microsoft.com/visualstudio/test/getting-started-with-unit-testing"/>
/// <br/><see href="https://docs.microsoft.com/visualstudio/test/using-microsoft-visualstudio-testtools-unittesting-members-in-unit-tests"/>
/// <br/><see href="https://docs.microsoft.com/visualstudio/test/run-unit-tests-with-test-explorer"/>
/// </summary>
[TestClass]
public class TestClass
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
    public void UiTestMethod()
    {
        Assert.AreEqual(0, new Grid().ActualWidth);
    }
}
