using System;
using System.Collections.Generic;

namespace Pooling
{
    /// <summary>
    /// Provides an implementation of the IObjectPool interface for managing the reuse of instances of type T.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the pool. Must be a subclass of UnityEngine.Object.</typeparam>
    public sealed class ObjectPooling<T> : IObjectPool<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// The delegate for creating a new instance of type T.
        /// </summary>
        private readonly Func<T> _createObject;

        /// <summary>
        /// The delegate for initializing an instance of type T before returning it from the pool.
        /// </summary>
        private readonly Action<T> _initObject;

        /// <summary>
        /// The delegate for recycling an instance of type T before releasing it to the pool.
        /// </summary>
        private readonly Action<T> _recycleObject;

        /// <summary>
        /// The event that is raised when an instance of type T is acquired from the pool.
        /// </summary>
        public event Action<T> OnObjectAcquired;

        /// <summary>
        /// The event that is raised when an instance of type T is released to the pool.
        /// </summary>
        public event Action<T> OnObjectReleased;

        /// <summary>
        /// The queue that stores the available instances of type T in the pool.
        /// </summary>
        private readonly Queue<T> _objectQueue;

        /// <summary>
        /// The lock object for thread-safety.
        /// </summary>
        private readonly object _lock = new();

        /// <summary>
        /// The maximum size of the pool. Zero means unlimited.
        /// </summary>
        private readonly uint _maxSize;

        /// <summary>
        /// Gets the current size of the pool, i.e. the number of instances in the queue.
        /// </summary>
        public int Size => _objectQueue.Count;

        /// <summary>
        /// Gets the number of available instances in the pool, i.e. the difference between the size and the number of instances in use.
        /// </summary>
        public int Available => _objectQueue.Count - InUse;

        /// <summary>
        /// Gets the number of instances that are currently in use, i.e. the difference between the size and the number of available instances.
        /// </summary>
        public int InUse => Size - _objectQueue.Count;

        /// <summary>
        /// Gets a value indicating whether the pool is empty, i.e. there are no available instances in the queue.
        /// </summary>
        public bool IsEmpty => _objectQueue.Count == 0;

        /// <summary>
        /// Gets a value indicating whether the pool is full, i.e. the size has reached the maximum limit.
        /// </summary>
        public bool IsFull => _maxSize > 0 && Size >= _maxSize;

        /// <summary>
        /// Creates a new instance of the ObjectPooling class with the specified parameters.
        /// </summary>
        /// <param name="createObject">A delegate that creates a new instance of type T.</param>
        /// <param name="initObject">A delegate that initializes an instance of type T when it is acquired from the pool.</param>
        /// <param name="recycleObject">A delegate that recycles an instance of type T when it is released to the pool.</param>
        /// <param name="initialValues">An optional collection of initial values to populate the pool.</param>
        /// <param name="initialSize">An optional parameter that specifies the initial size of the pool.</param>
        /// <param name="maxSize">An optional parameter that specifies the maximum size of the pool.</param>
        /// <exception cref="ArgumentNullException">Thrown when createObject is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when initialSize is negative.</exception>
        public ObjectPooling(Func<T> createObject,
            Action<T> initObject = null, Action<T> recycleObject = null,
            IEnumerable<T> initialValues = null,
            int initialSize = 10, uint maxSize = 1000)
        {
            _createObject = createObject ?? throw new ArgumentNullException(
                nameof(createObject),
                "The createObject cannot be null");

            _initObject = initObject;
            _recycleObject = recycleObject;

            if (initialSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialSize), "The initialSize cannot be negative");
            }

            _maxSize = maxSize;
            _objectQueue = initialValues != null ? new Queue<T>(initialValues) : CreateInitialQueue(initialSize);
        }

        private Queue<T> CreateInitialQueue(int size)
        {
            var queue = new Queue<T>(size);
            for (var i = 0; i < size; i++)
                queue.Enqueue(_createObject());

            return queue;
        }

        /// <summary>
        /// Gets an instance of type T from the pool. If the pool is empty, creates a new instance.
        /// </summary>
        /// <returns>An instance of type T.</returns>
        public T Get()
        {
            lock (_lock)
            {
                if (!_objectQueue.TryDequeue(out var obj))
                {
                    obj = _createObject?.Invoke();
                }

                _initObject?.Invoke(obj);
                return obj;
            }
        }

        /// <summary>
        /// Releases an instance of type T back to the pool. If the pool is full, destroys the instance.
        /// </summary>
        /// <param name="obj">The instance to release.</param>
        /// <exception cref="ArgumentNullException">Thrown when obj is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when obj is already in the pool.</exception>
        public void Release(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object cannot be null");
            }

            lock (_lock)
            {
                if (_objectQueue.Contains(obj))
                {
                    throw new InvalidOperationException(
                        "Problem while releasing object: Element which you want to release is already released.");
                }

                if (Size < _maxSize)
                {
                    _objectQueue.Enqueue(obj);
                }

                _recycleObject?.Invoke(obj);
            }
        }
    }
}