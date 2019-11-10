using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamSeifert.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamSeifert.Tests.Utilities.DataStructures
{
    [TestClass]
    public class HeapTests
    {
        [TestMethod]
        public void RightOrder()
        {
            var heap = new Heap<int>();

            heap.Add(100);
            heap.Add(23);
            heap.Add(163);
            heap.Add(3);
            heap.Add(32476);
            heap.Add(1236123);

            Assert.AreEqual(3, heap.Peek());
        }

        [TestMethod]
        public void RightOrderReversed()
        {
            var heap = new Heap<int>(true);

            heap.Add(100);
            heap.Add(23);
            heap.Add(163);
            heap.Add(3);
            heap.Add(32476);
            heap.Add(1236123);

            Assert.AreEqual(1236123, heap.Peek());
        }
    }
}
