using System;

public class LimitedQueue
{
	private object[] array;
	private int head;
	private int tail;
	private int size;
	
	public LimitedQueue (int size)
	{
		size--;
		size |= size >> 1;
		size |= size >> 2;
		size |= size >> 4;
		size |= size >> 8;
		size |= size >> 16;
		this.size = ++size;
		array = new object[size];
	}
	
	public void Enqueue(object obj) {
		array[tail] = obj;
		tail = (tail + 1) & size - 1;
	}
	public object Dequeue() {
		object ret = array[head];
		head = (head + 1) & size - 1;
		return ret;
	}
	public int Count {
		get {
			return (tail - head) & size - 1;
		}
	}
}

