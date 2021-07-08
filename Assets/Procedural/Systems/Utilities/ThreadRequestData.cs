using System;

namespace Procedural.Utilities
{
	public class ThreadRequestData<T>
	{
		public readonly Action<T> callback;

		public readonly T requestedData;

		public ThreadRequestData(Action<T> callback, T requestedData)
		{
			this.callback = callback;
			this.requestedData = requestedData;
		}
	}
}