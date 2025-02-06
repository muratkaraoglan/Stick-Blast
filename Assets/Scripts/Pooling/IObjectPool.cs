namespace  Pooling
{
    /// <summary>
    /// Defines an interface for an object pool that manages the reuse of instances of type T.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the pool. Must be a subclass of UnityEngine.Object.</typeparam>
    public interface IObjectPool<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// Gets an instance of type T from the pool. If the pool is empty, creates a new instance.
        /// </summary>
        /// <returns>An instance of type T.</returns>
        T Get();

        /// <summary>
        /// Releases an instance of type T back to the pool. If the pool is full, destroys the instance.
        /// </summary>
        /// <param name="element">The instance to release.</param>
        void Release(T element);
    }
}