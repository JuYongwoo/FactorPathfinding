using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> _heap;

    public PriorityQueue(int capacity = 0)
    {
        _heap = new List<T>(capacity);  // 초기 용량 설정
    }

    // O(logN)
    public void push(T data)
    {
        _heap.Add(data);  // 힙의 끝에 데이터를 추가
        int now = _heap.Count - 1;  // 새 데이터의 위치

        // 부모와 비교하면서 거슬러 올라가는 방식으로 힙 속성 유지
        while (now > 0)
        {
            int parent = (now - 1) / 2;
            if (_heap[now].CompareTo(_heap[parent]) <= 0)
                break;  // 부모보다 작거나 같으면 종료

            // 부모와 자식을 교환 (swap 대신 직접 교체)
            T temp = _heap[now];
            _heap[now] = _heap[parent];
            _heap[parent] = temp;

            now = parent;  // 부모로 이동
        }
    }

    // O(logN)
    public T pop()
    {

        T ret = _heap[0];  // 루트 값 저장

        // 마지막 데이터를 루트로 이동
        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);

        int now = 0;
        // 루트에서 아래로 내려가며 힙 속성을 유지
        while (true)
        {
            int left = 2 * now + 1;
            int right = 2 * now + 2;
            int largest = now;

            // 왼쪽 자식이 더 크면 왼쪽 자식 선택
            if (left < _heap.Count && _heap[largest].CompareTo(_heap[left]) < 0)
                largest = left;
            // 오른쪽 자식이 더 크면 오른쪽 자식 선택
            if (right < _heap.Count && _heap[largest].CompareTo(_heap[right]) < 0)
                largest = right;

            if (largest == now)
                break;  // 자식들이 모두 현재 노드보다 작으면 종료

            // 현재 노드와 자식을 교환
            T temp = _heap[now];
            _heap[now] = _heap[largest];
            _heap[largest] = temp;

            now = largest;  // 이동
        }

        return ret;
    }

    public int Count { get { return _heap.Count; } }
}
