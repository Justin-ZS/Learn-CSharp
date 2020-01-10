// 具体要求：
// 2. 至少对IList中的Add，Remove，Insert，Indexer，Ienumerator进行单元测试。
// 3. 对上述每个单元测试方法至少书写4种不同的单元测试。

using NUnit.Framework;
using System;
using List;

namespace List.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Construct_List()
        {
            var emptyList = new LinkedList<int>();
            Assert.AreEqual(0, emptyList.Count);

            var list = new LinkedList<int>(1, 2, 3);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(1)]
        [TestCase("foobar")]
        public void Add_Value_Type_To_List<T>(T value)
        {
            var list = new LinkedList<T>();
            Assert.AreEqual(0, list.Count);

            list.Add(value);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(value, list[0]);

            list.Add(default(T));
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(default(T), list[1]);
        }
        [Test]
        public void Test_Ref_Type_To_List() {
            int[] arr = { 1, 2 };
            var list = new LinkedList<int[]>();
            list.Add(arr);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(arr, list[0]);
            Assert.IsTrue(list.Remove(arr));
            Assert.AreEqual(0, list.Count);
        }

        [TestCase(1)]
        [TestCase("foobar")]
        public void Remove_Item_From_List_Failure<T>(T value)
        {
            var list = new LinkedList<T>();
            Assert.AreEqual(0, list.Count);
            Assert.IsFalse(list.Remove(value));
            Assert.AreEqual(0, list.Count);

            list.Add(default(T));
            Assert.AreEqual(1, list.Count);
            Assert.IsFalse(list.Remove(value));
            Assert.AreEqual(1, list.Count);
        }
        [TestCase(1)]
        [TestCase("foobar")]
        public void Remove_Item_From_List_Success<T>(T value)
        {
            var list = new LinkedList<T>();
            list.Add(value);
            list.Add(value);
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Remove(value));
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void RemoveAt_List()
        {
            var list = new LinkedList<int>(1, 2);
            Assert.AreEqual(2, list.Count);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.RemoveAt(-1));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.RemoveAt(2));

            list.RemoveAt(1);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.RemoveAt(1));

            list.RemoveAt(0);
            Assert.AreEqual(0, list.Count);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.RemoveAt(0));
        }
        [Test]
        public void Insert_Item_Into_List()
        {
            var list = new LinkedList<int>(1, 2);
            Assert.AreEqual(2, list.Count);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.Insert(-1, 100));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.Insert(3, 100));

            list.Insert(1, 100);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(100, list[1]);
            Assert.AreEqual(String.Join(" ", list), "1 100 2");
            // insert to the last
            list.Insert(list.Count, 300);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(300, list[list.Count - 1]);
            Assert.AreEqual(String.Join(" ", list), "1 100 2 300");
        }
        [Test]
        public void Indexer_Of_List()
        {
            var list = new LinkedList<int>(1, 2, 3);
            Assert.AreEqual(3, list.Count);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { var a = list[-1]; });
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { list[-1] = 1; });
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);

            list[1] = 100;
            Assert.AreEqual(100, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { var a = list[3]; });
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { list[list.Count] = 333; });
        }
        [Test]
        public void Iterate_List()
        {
            var list = new LinkedList<int>(1, 2, 3);
            Assert.AreEqual(String.Join(" ", list), "1 2 3");

            var enumerator = list.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());

            var emptyList = new LinkedList<int>();
            var enumerator3 = emptyList.GetEnumerator();
            Assert.IsFalse(enumerator3.MoveNext());
        }
        [Test]
        public void Iterate_List_After_Changed()
        {
            var list = new LinkedList<int>(1, 2);

            var enumerator = list.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());

            enumerator = list.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            list.Add(3);
            Assert.Throws(typeof(InvalidOperationException), () => enumerator.MoveNext());

            enumerator = list.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            list.Remove(1);
            Assert.Throws(typeof(InvalidOperationException), () => enumerator.MoveNext());

            enumerator = list.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());

            list.Clear();
            Assert.Throws(typeof(InvalidOperationException), () => enumerator.MoveNext());
        }
        [Test]
        public void Clear_List()
        {
            var list = new LinkedList<int>(1, 2);
            Assert.AreEqual(2, list.Count);
            
            list.Clear();
            Assert.AreEqual(0, list.Count);
        }
        [Test]
        public void Contains_List()
        {
            var list = new LinkedList<int>(1, 2);
            Assert.IsFalse(list.Contains(0));
            Assert.IsTrue(list.Contains(1));
            Assert.IsTrue(list.Contains(2));
            Assert.IsFalse(list.Contains(3));

            int[] arr = { 1, 2 };
            int[] arr2 = { 1, 2 };
            var list2 = new LinkedList<int[]>(arr);

            Assert.IsTrue(list2.Contains(arr));
            Assert.IsFalse(list2.Contains(arr2));
        }
        [Test]
        public void IndexOf_List()
        {
            var list = new LinkedList<int>(1, 2, 2);
            Assert.AreEqual(-1, list.IndexOf(0));
            Assert.AreEqual(0, list.IndexOf(1));
            Assert.AreEqual(1, list.IndexOf(2));
            Assert.AreEqual(-1, list.IndexOf(3));

            int[] arr = { 1, 2 };
            int[] arr2 = { 1, 2 };
            var list2 = new LinkedList<int[]>(arr);
            Assert.AreEqual(0, list2.IndexOf(arr));
            Assert.AreEqual(-1, list2.IndexOf(arr2));

            list2.Insert(1, arr2);
            Assert.AreEqual(1, list2.IndexOf(arr2));

            list2.Insert(0, arr2);
            Assert.AreEqual(0, list2.IndexOf(arr2));
        }
        [Test]
        public void CopyTo_List()
        {
            var list = new LinkedList<int>(11, 22);
            int[] arr = { 1, 2, 3 ,4 };
            
            Assert.Throws(typeof(ArgumentNullException), () => list.CopyTo(null, 0));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.CopyTo(arr, -1));
            Assert.Throws(typeof(ArgumentException), () => list.CopyTo(new int[]{ 1 }, 0));

            list.CopyTo(arr, 0);
            Assert.AreEqual(arr, new[] { 11, 22, 3, 4 });
            list.CopyTo(arr, 2);
            Assert.AreEqual(arr, new[] { 11, 22, 11, 22 });
        }
    }
}