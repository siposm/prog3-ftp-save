using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Lecture_Stack
{
    public class MyStack
    {
        List<int> numbers = new List<int>();
        public void Push(int num)
        {
            numbers.Add(num);
        }
        public int Pop()
        {
            int num = numbers[numbers.Count-1];
            numbers.RemoveAt(numbers.Count - 1);
            return num;
        }
    }

    // NUGET: Add nUnit, nUnit3TestAdapter, Microsoft.NET.Test.Sdk
    [TestFixture]
    public class StackTester
    {
        [Test]
        public void TestTrue()
        {
            Assert.That(true, Is.True);
        }

        MyStack stack;
        [SetUp]
        public void Setup()
        {
             stack = new MyStack();
        }

        [Test]
        public void TestSingleAdd()
        {
            stack.Push(42);
            int num = stack.Pop();
            Assert.That(num, Is.EqualTo(42));
        }

        [TestCase(new int[] { 13, 23 }, new int[] { 23, 13 })]
        [TestCase(new int[] { -15, 0, 42 }, new int[] { 42, 0, -15 })]
        public void TestSequence(int[] input, int[] output)
        {
            foreach (int num in input) stack.Push(num);

            foreach (int num in output)
            {
                Assert.That(stack.Pop(), Is.EqualTo(num));
            }
        }
    }
}
