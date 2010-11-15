using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bot
{
	/// <summary>
    /// An enumeration of all the permuations of a set of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AllPermutationsEnumerable<T> : IEnumerable<IEnumerable<T>>
    {
        /// <summary>The set of items over which all permutations are generated.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IEnumerable<T> items;
 
        /// <summary>
        /// Creates and initializes a new instance of the <see cref="AllPermutationsEnumerable{T}"/> type.
        /// </summary>
        /// <param name="items">The original set of items over which all permutations are generated.</param>
        public AllPermutationsEnumerable(IEnumerable<T> items)
            : base()
        {
            this.items = items;
        }
 
        /// <summary>
        /// Gets an enumerator to enumerate over the items in this enumeration.
        /// </summary>
        /// <returns>An instance of <see cref="IEnumerator{T}"/>.</returns>
        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return new AllPermutationsEnumerator<T>(this.items);
        }
 
        /// <summary>
        /// Gets an enumerator to enumerate over the items in this enumeration.
        /// </summary>
        /// <returns>An instance of <see cref="IEnumerator{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new AllPermutationsEnumerator<T>(this.items);
        }
    }

    /// <summary>
    /// Enumerators over all possible permutations of a set of items.
    /// </summary>
    /// <typeparam name="T">The type of the items in the original set.</typeparam>
    public class AllPermutationsEnumerator<T> : IEnumerator<IEnumerable<T>>
    {
        /// <summary>The set of items over which all permutations are generated.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private T[] originalItems;
 
        /// <summary>Flag to indicate if this enumerator is in its initial state.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isFirst;
 
        /// <summary>Flag to indicate if this enumerator has been disposed of.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isDisposed;
 
        /// <summary>The current permutation.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private T[] current;
 
        /// <summary>The indicates of the items in the original set that are selected for the current permutation.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int[] indices;
 
        /// <summary>A state variable used by the permutation algorithm.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool even;
 
        /// <summary>A state variable used by the permutation algorithm to track whether this enumerator is finished.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isFinished;
 
        /// <summary>
        /// Creates and initializes an instance of the <see cref="AllPermutationsEnumerator{T}"/> type.
        /// </summary>
        /// <param name="items">The set of items over which permutations are to be created.</param>
        public AllPermutationsEnumerator(IEnumerable<T> items)
            : base()
        {
            if (null == items)
            {
                throw new ArgumentNullException("items");
            }
 
            this.originalItems = items.ToArray();
            this.indices = new int[this.originalItems.Length];
            for (int i = 0; i < this.indices.Length; ++i)
            {
                this.indices[i] = i + 1;
            }
            this.current = null;
            this.even = true;
            this.isDisposed = false;
            this.isFirst = true;
            this.isFinished = false;
        }
 
        /// <summary>
        /// Gets the current item referred to by this enumerator.
        /// </summary>
        public IEnumerable<T> Current
        {
            get
            {
                this.CheckIfDisposed();
 
                return this.current;
            }
        }
 
        /// <summary>
        /// Disposes of this enumerator.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }
 
            this.isDisposed = true;
            this.originalItems = null;
            this.current = null;
            this.indices = null;
            this.even = true;
            this.isFirst = true;
            this.isFinished = false;
        }
 
        /// <summary>
        /// Gets the current item referred to by this enumerator.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
 
        /// <summary>
        /// Moves this enumerator to refer to the next item in the enumeration.
        /// </summary>
        /// <returns><c>true</c> if a value is available; <c>false</c> if the end has been reached.</returns>
        public bool MoveNext()
        {
            this.CheckIfDisposed();
 
            if (this.isFinished)
            {
                return false;
            }
 
            if (this.isFirst)
            {
                this.even = true;
                this.isFirst = false;
                this.Select();
                return true;
            }
            else if (this.originalItems.Length == 1)
            {
                this.current = null;
                return false;
            }
 
            if (even)
            {
                this.even = false;

                //this.indices.Swap(0, 1);
				int tmp = indices[0];
            	indices[0] = indices[1];
            	indices[1] = tmp;

                this.Select();
 
                if (!((this.indices[this.indices.Length-1] != 1) || (this.indices[0] != (2 + this.indices.Length % 2))))
                {
                    if (this.indices.Length <= 3)
                    {
                        this.isFinished = true;
                    }
                    else
                    {
                        this.isFinished = true;
                        for (int i = 0 ; i < (this.indices.Length - 3); ++i)
                        {
                            if (this.indices[i+1] != this.indices[i]+1)
                            {
                                this.isFinished = false;
                            }
                        }
                    }
                }
                return true;
            }
 
            int s = 0;
            this.even = true;
 
            for (int k = 1; k < this.indices.Length; ++k)
            {
                int d = 0;
                int ia = this.indices[k];
                int i = k - 1;
 
                for (int j = 0; j <= i; ++j)
                {
                    if (this.indices[j] > ia)
                    {
                        ++d;
                    }
                }
                s += d;
 
                if (d != (i+1) * (s % 2))
                {
                    int l = 0;
                    int m = ((s + 1) % 2) * (this.indices.Length + 1);
                    for (int j = 0; j <= i; ++j)
                    {
                        if (this.Sign(1, this.indices[j] - ia) != this.Sign(1, this.indices[j] - m))
                        {
                            m = this.indices[j];
                            l = j;
                        }
                    }
                    this.indices[l] = ia;
                    this.indices[k] = m;
                    this.even = true;
                    this.Select();
                    break;
                }
            }
 
            return true;
        }
 
        /// <summary>
        /// Simulates the fortran SIGN function that computes the absolute of the parameter <paramref name="x"/>
        /// and then assigns the sign of the parameter <paramref name="y"/> to <paramref name="x"/> and returns
        /// this new value.
        /// </summary>
        /// <param name="x">The value to sign.</param>
        /// <param name="y">The value that determines the sign.</param>
        /// <returns><paramref name="x"/> signed by <paramref name="y"/>.</returns>
        private int Sign(int x, int y)
        {
            if (y >= 0)
            {
                x = Math.Abs(x);
            }
            else
            {
                x = -1 * Math.Abs(x);
            }
 
            return x;
        }
 
        /// <summary>
        /// Selects the items in the current permutation based on the computed indicies of the items
        /// to be selected.
        /// </summary>
        private void Select()
        {
            this.current = new T[this.originalItems.Length];
            for (int i = 0; i < this.indices.Length; ++i)
            {
                this.current[i] = this.originalItems[this.indices[i] - 1];
            }
        }
 
        /// <summary>
        /// Resets this enumerator to the initial state.
        /// </summary>
        public void Reset()
        {
            this.CheckIfDisposed();
 
            for (int i = 0; i < this.indices.Length; ++i)
            {
                this.indices[i] = i + 1;
            }
 
            this.current = null;
            this.even = true;
            this.isDisposed = false;
            this.isFirst = true;
            this.isFinished = false;
        }
 
        /// <summary>
        /// Checks and throws an exception if this enumerator has been disposed.
        /// </summary>
        private void CheckIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
