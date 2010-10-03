using System;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Utility class that simplifies the usage of IDisposable
	/// </summary>
	public abstract class Disposable : IDisposable
	{
		private bool isDisposed = false;

		/// <summary>
		/// Returns true if this instance has been disposed of, false otherwise
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Disposable"/> is reclaimed by garbage collection.
		/// </summary>
		~Disposable()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		public void Dispose()
		{
			if (!isDisposed)
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources. Rember to call this method from your derived class.
		/// </summary>
		/// <param name="disposing">
		/// Set to <c>true</c> to release both managed and unmanaged resources.
		/// Set to <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
				isDisposed = true;
		}
	}
}