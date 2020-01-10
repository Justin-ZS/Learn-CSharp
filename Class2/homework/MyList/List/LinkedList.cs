// 实现链表（LinkedList）
// 简介：写一个链表的数据结构，要求实现IList<T>接口。

// 具体要求：
// 1. 使用代码规范。
// 2. 至少对IList中的Add，Remove，Insert，Indexer，IEnumerator进行单元测试。
// 3. 对上述每个单元测试方法至少书写4种不同的单元测试。
// 4. 要求从Object派生，实现System.Collections.Generic.IList<T>。
// 5. 内部存储不能使用.NET内置链表。

// 注意事项：
// 1. 单元测试采用Visual Studio Team Edition内置UnitTest
// 2. 对方法的参数要进行检查，并抛出合理的Exception。

// 考查目的：
// 1. C#语法。
// 2. 常用的类库。
// 3. 代码规范。
// 4. 单元测试。

using System;
using System.Collections;
using System.Collections.Generic;

namespace List
{
    public class LinkedList<T> : IList<T>
    {
        private Node Head;
        private Node Tail;
        private int _count;
        public int Count
        {
            get => _count;
            private set
            {
                _count = value;
                _OnChanged();
            }
        }
        public bool IsReadOnly => false;
        public LinkedList()
        {
            _Init();
        }
        public LinkedList(params T[] xs)
        {
            _Init();
            Array.ForEach(xs, x => this.Add(x));
        }
        public void Add(T value)
        {
            _InsertBefore(Tail, value);
        }
        public void Clear()
        {
            _Init();
        }
        public bool Contains(T value)
        {
            int index = _GetIndexByValue(value);
            return index != -1;
        }
        public void CopyTo(T[] xs, int index)
        {
            if (xs == null)
            {
                throw new ArgumentNullException("array");
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            if (Count > xs.Length - index)
            {
                throw new ArgumentException("array");
            }

            var curIndex = index;
            foreach (var value in this)
            {
                xs[curIndex] = value;
                curIndex++;
            }
        }
        public bool Remove(T value)
        {
            int index = _GetIndexByValue(value);
            bool success = _TryRemoveAt(index);
            return success;
        }
        public IEnumerator<T> GetEnumerator()
        {
            var isListChanged = false;
            EventHandler onListChanged = (sender, e) => isListChanged = true;
            Changed += onListChanged;

            try
            {
                var curNode = Head.Next;
                while (curNode != Tail)
                {
                    if (isListChanged)
                    {
                        throw new InvalidOperationException("The collection was modified after the enumerator was created.");
                    }
                    yield return curNode.Value;
                    curNode = curNode.Next;
                }
            }
            finally
            {
                Changed -= onListChanged;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public T this[int index]
        {
            get
            {
                var node = _TryGetNodeByValidIndex(index);
                if (node == null)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return node.Value;
            }
            set
            {
                var node = _TryGetNodeByValidIndex(index);
                if (node == null)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                node.Value = value;
                _OnChanged();
            }
        }
        public int IndexOf(T value)
        {
            return _GetIndexByValue(value);
        }
        public void Insert(int index, T value)
        {
            bool success = _TryInsertAt(index, value);
            if (!success)
            {
                throw new ArgumentOutOfRangeException("index");
            }
        }
        public void RemoveAt(int index)
        {
            bool success = _TryRemoveAt(index);
            if (!success)
            {
                throw new ArgumentOutOfRangeException("index");
            }
        }

        public event EventHandler Changed;
        private void _OnChanged() => Changed?.Invoke(this, EventArgs.Empty);

        private void _Init()
        {
            Head = new Node();
            Tail = new Node();

            Head.Next = Tail;
            Tail.Prev = Head;

            Count = 0;
        }
        private bool _IsIndexOutOfRange(int index)
        {
            // A valid index should be within the bounds of the List
            return index < 0 || index >= Count;
        }
        private int _GetIndexByValue(T value)
        {
            var curNode = Head.Next;
            int index = 0;
            while (curNode != Tail)
            {
                if (EqualityComparer<T>.Default.Equals(curNode.Value, value))
                {
                    return index;
                }

                index++;
                curNode = curNode.Next;
            }

            return -1;
        }
        private Node _GetNodeByIndex(int index)
        {
            if (index == -1)
            {
                return Head;
            }
            if (index == Count)
            {
                return Tail;
            }
            if (_IsIndexOutOfRange(index))
            {
                return null;
            }

            bool isCloseToHead = index <= (Count / 2);
            var startNode = Head;
            var endNode = Tail;
            var dir = Node.Direction.Next;
            int idx = index;

            if (!isCloseToHead)
            {
                startNode = Tail;
                endNode = Head;
                dir = Node.Direction.Prev;
                idx = Count - 1 - index;
            }

            var curNode = startNode.GetAdjacentNode(dir);
            while (curNode != endNode && idx != 0)
            {
                curNode = curNode.GetAdjacentNode(dir);
                idx--;
            }

            if (curNode == endNode)
            {
                return null; // ??? => _TryGetNodeByValidIndex
            }
            return curNode;
        }
        private Node _TryGetNodeByValidIndex(int index)
        {
            var node = _GetNodeByIndex(index);
            if (_IsIndexOutOfRange(index) || node == null)
            {
                return null;
            }
            return node;
        }

        private void _InsertAfter(Node node, T value)
        {
            var newAdded = new Node(value);
            var nextNode = node.Next;

            node.Next = newAdded;
            newAdded.Prev = node;

            newAdded.Next = nextNode;
            nextNode.Prev = newAdded;

            Count++;
        }
        private void _InsertBefore(Node node, T value)
        {
            _InsertAfter(node.Prev, value);
        }
        private bool _TryInsertAt(int index, T value)
        {
            var node = _GetNodeByIndex(index);
            if (node == null || node == Head)
            {
                return false;
            }

            _InsertBefore(node, value);
            return true;
        }

        private void _RemoveAfter(Node node)
        {
            if (node == Tail || node.Next == Tail)
            {
                return;
            }

            var nodeAfterRemove = node.Next.Next;
            node.Next = nodeAfterRemove;
            nodeAfterRemove.Prev = node;

            Count--;
        }
        private void _RemoveBefore(Node node)
        {
            if (node == Head || node.Prev == Head)
            {
                return;
            }

            _RemoveAfter(node.Prev.Prev);
        }
        private bool _TryRemoveAt(int index)
        {
            var node = _TryGetNodeByValidIndex(index);
            if (node == null)
            {
                return false;
            }

            _RemoveAfter(node.Prev);
            return true;
        }

        private void _LogAllNodes()
        {
            var curNode = Head.Next;
            Console.WriteLine("Start log all nodes: -----");
            while (curNode != Tail)
            {
                Console.WriteLine(curNode.Value);
                curNode = curNode.Next;
            }
            Console.WriteLine("End log all nodes: -----");
        }

        private class Node
        {
            public Node() { }
            public Node(T value)
            {
                Value = value;
            }

            public Node Prev { get; set; }
            public Node Next { get; set; }
            public T Value { get; set; }

            public enum Direction
            {
                Next,
                Prev,
            }
            public Node GetAdjacentNode(Direction dir)
            {
                return dir == Direction.Next ? Next : Prev;
            }
        }
        private class NodeEnumerator : IEnumerator<T>
        {
            private bool _listChanged = false;
            private Node _curNode { get; set; }
            private int _curIndex { get; set; }
            readonly private LinkedList<T> _linkedList;
            private void _OnChanged(object sender, EventArgs e)
            {
                _listChanged = true;
            }

            public NodeEnumerator(LinkedList<T> linkedList)
            {
                _linkedList = linkedList;
                _linkedList.Changed += _OnChanged;
                Reset();
            }
            public void Reset()
            {

                // Sets the enumerator to its initial position,
                // which is before the first element in the collection.
                _curIndex = -1;
                _curNode = _linkedList.Head;
            }
            public bool MoveNext()
            {
                // An enumerator remains valid as long as the collection remains unchanged.
                if (_listChanged)
                {
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");
                }
                if(_curNode == _linkedList.Tail)
                {
                    return false;
                }
                // If MoveNext passes the end of the collection,
                // the enumerator is positioned after the last element in the collection
                // and MoveNext returns false. 
                _curNode = _curNode.Next;

                return _curNode != _linkedList.Tail;
            }
            public T Current
            {
                get => _curNode.Value;
            }

            object IEnumerator.Current
            {
                get => Current;
            }
            void IDisposable.Dispose()
            {
                _linkedList.Changed -= _OnChanged;
            }
        }
    }
}