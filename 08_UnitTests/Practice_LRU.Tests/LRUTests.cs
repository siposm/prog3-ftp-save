using NUnit.Framework;
using Practice_LRU.Logic;
using System;
using System.Collections.Generic;

namespace Practice_LRU.Tests
{
    // NUGET: Add nUnit, nUnit3TestAdapter, Microsoft.NET.Test.Sdk + Project Reference
    [TestFixture]
    public class LRUTests
    {
        [Test]
        public void TestCtor()
        {
            // AAA = Arrange, Act, Assert (result, exception, state, circumstance)
            LRU storage = new LRU();
            Assert.NotNull(storage.Recent); // old syntax
            Assert.That(storage.Recent, Is.Not.Null);
            Assert.That(storage.Recent, Is.Empty);
        }

        [Test]
        public void TestDefaultCtorParameter()
        {
            LRU storage = new LRU();
            Assert.That(storage.ListLimit, Is.EqualTo(LRU.DEFAULT_LIMIT));
        }

        [TestCase(2)]
        [TestCase(42)]
        [TestCase(20000)]
        public void TestCtorLimitParameter(int limit)
        {
            LRU storage = new LRU(limit);
            Assert.That(storage.ListLimit, Is.EqualTo(limit));
        }
        // boundary tests
        // random testing: monkey testing, fuzzing

        [TestCase(0)]
        [TestCase(-42)]
        public void TestCtorBadLimitRefused(int limit) // 2.
        {
            Assert.That(
                () => { LRU storage = new LRU(limit); },
                Throws.TypeOf<ArgumentOutOfRangeException>()
            );
        }

        const int LIMIT = 5;
        LRU lru;
        [SetUp]
        public void Setup()
        {
            lru = new LRU(LIMIT);
        }
        [Test] // 3.
        public void TestAddNullShouldThrowException()
        {
            Assert.That(() => lru.Add(null), Throws.ArgumentNullException);
        }
        [Test] // 4.
        public void TestLimitWorks()
        {
            object[] instances = new object[2 * LIMIT];
            for (int i = 0; i < instances.Length; i++)
            {
                instances[i] = new { IDX = i };
                lru.Add(instances[i]);
            }
            Assert.That(lru.Recent.Count, Is.EqualTo(LIMIT));
            for (int i = 0; i < LIMIT; i++)
            {
                Assert.That(lru.Recent, Does.Contain(instances[i + LIMIT]));
            }
        }

        // ***NO*** IEnumarable<object[]> since nunit/issues/1792
        static IEnumerable<TestCaseData> TestCountsSource
        {
            get
            {
                var first = new { IDX = 1 };
                var second = new { IDX = 2 };
                var third = new { IDX = 3 };
                List<TestCaseData> output = new List<TestCaseData>();
                output.Add(new TestCaseData(new object[] { first, second, third }, 3));
                output.Add(new TestCaseData(new object[] { first, second, second }, 2));
                output.Add(new TestCaseData(new object[] { first, first, first }, 1));
                return output;
                // alternative: yield return
            }
        }
        [TestCaseSource(nameof(TestCountsSource))] // 5.
        public void TestCounts(object[] instances, int expectedCount)
        {
            foreach (object item in instances) lru.Add(item);
            Assert.That(lru.Recent.Count, Is.EqualTo(expectedCount));
        }
        
        public static IEnumerable<TestCaseData> TestOrderSource
        {
            get
            {
                var first = new { IDX = 1 };
                var second = new { IDX = 2 };
                var third = new { IDX = 3 };
                List<TestCaseData> output = new List<TestCaseData>();
                output.Add(new TestCaseData
                (
                    new object[] { first, second, third }, // items added
                    new object[] { third, second, first } // expected order
                ));
                output.Add(new TestCaseData
                (
                    new object[] { first, second, second }, // items added
                    new object[] { second, first } // expected order
                ));
                output.Add(new TestCaseData
                (
                    new object[] { first, second, first }, // items added
                    new object[] { first, second } // expected order
                ));
                return output;
            }
        }
        [TestCaseSource(nameof(TestOrderSource))] // 6.
        public void TestOrder(object[] instancesToAdd, object[] expectedOrder)
        {
            foreach (object item in instancesToAdd) lru.Add(item);
            for (int i = 0; i < expectedOrder.Length; i++)
            {
                Assert.That(lru.Recent[i], Is.SameAs(expectedOrder[i]));
            }
            // REGRESSION: fixes in #7
        }
    }

}
