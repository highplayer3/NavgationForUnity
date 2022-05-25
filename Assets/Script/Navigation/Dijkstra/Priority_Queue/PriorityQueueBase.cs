using System.Collections;
using System.Collections.Generic;
using System;

namespace CPPSTL
{
    sealed class MyCom : IComparer<Point>
    {
        public int Compare(Point p1, Point p2)
        {
            return p2.F.CompareTo(p1.F);
        }
    }
    sealed class MyComPosX : IComparer<Point>
    {
        public int Compare(Point p1, Point p2)
        {
            return p2.x.CompareTo(p1.x);
        }
    }
    sealed class MyComPosY : IComparer<Point>
    {
        public int Compare(Point p1, Point p2)
        {
            return p2.y.CompareTo(p1.y);
        }
    }


    sealed class PriorityQueue<T>:IEnumerable<T>
    {
        IComparer<T> comparer;
        IComparer<T> comparer1;
        IComparer<T> comparer2;
        private T[] heap;

        public int Count { get; private set; }

        public PriorityQueue() : this(null) { }
        
        public PriorityQueue(IComparer<T> comparer) : this(16, comparer) { }

        public PriorityQueue(IComparer<T> comparer,IComparer<T> _comparer1, IComparer<T> _comparer2) : this(16,comparer)
        {
            this.comparer1 = _comparer1;
            this.comparer2 = _comparer2;
        }
        public PriorityQueue(T[] arr,IComparer<T> comparer)
        {
            for(int i = 0; i < arr.Length; i++)
            {
                this.Push(arr[i]);
                Count++;
            }
            this.comparer = comparer;
            
        }

        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            this.comparer = (comparer == null) ? Comparer<T>.Default : comparer;
            this.heap = new T[capacity];
        }

        public void Push(T v)
        {
            if (Count >= heap.Length) Array.Resize(ref heap, Count * 2);
            heap[Count] = v;
            heapInsert(Count++);
        }

        public T Pop()
        {
            var v = Top();
            heap[0] = heap[--Count];//把末尾的那个放到堆顶，然后heapify
            if (Count > 0) heapify(0);
            return v;
        }

        public T Top()
        {
            if (Count > 0) return heap[0];
            throw new InvalidOperationException("优先队列为空");
        }

        public T Contains(T v)
        {
            foreach(var po in heap)
            {
                if (comparer1.Compare(po, v) == 0&&comparer2.Compare(po,v)==0)
                {
                    return po;
                }
            }
            return default;

        }
        //这个方法专门用来解决用堆来优化A*算法的更新点时
        public void UpdateHeap(T v)
        {
            int index=0;
            for(int i = 0; i < Count; i++)
            {
                //通过自定义的比较器来验证相等，也可以直接heap[i].x==v.x&&heap[i].y==v.y来写
                if (comparer1.Compare(heap[i], v) == 0&&comparer2.Compare(heap[i],v)==0)
                {
                    index = i;
                    break;
                }
            }
            heapInsert(index);//更新后F值一定变小，所以要向上冒
        }
        void heapInsert(int n)
        {
            var v = heap[n];//保存当前的值
            //向上冒
            for (var n2 = n/ 2; n > 0 && comparer.Compare(v, heap[n2]) > 0; n = n2, n2 =n/2) heap[n] = heap[n2];
            heap[n] = v;
            
        }

        void heapify(int n)
        {
            var v = heap[n];
            //向下冒，注意左右孩子还得比较
            for (var n2 = n * 2; n2 < Count; n = n2, n2 *= 2)
            {
                if (n2 + 1 < Count && comparer.Compare(heap[n2 + 1], heap[n2]) > 0) n2++;//左右孩子之间的比较
                if (comparer.Compare(v, heap[n2]) >= 0) break;//孩子节点和父亲节点间的比较
                heap[n] = heap[n2];
            }
            heap[n] = v;
            
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return heap.GetEnumerator();
        }

        IEnumerator<T> GetEnumerator()
        {
           for(int index = 0; index < Count; index++)
            {
                yield return heap[index];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
           return GetEnumerator();
        }
    }
}