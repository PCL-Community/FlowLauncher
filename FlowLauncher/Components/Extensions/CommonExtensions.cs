namespace FlowLauncher.Components.Extensions;

public static class CommonExtensions
{
    extension<T>(T generic)
    {
        /// <summary>
        /// Invoke <paramref name="func"/> by generic value.
        /// </summary>
        /// <param name="func">The function to invoke.</param>
        public void Let(Action<T> func) => func(generic);

        /// <summary>
        /// Invoke <paramref name="func"/> by generic value and return the result.
        /// </summary>
        /// <param name="func">The function to invoke.</param>
        /// <typeparam name="TReturn">Return value type of <paramref name="func"/>.</typeparam>
        /// <returns>Result of <paramref name="func"/>.</returns>
        public TReturn Let<TReturn>(Func<T, TReturn> func) => func(generic);

        /// <summary>
        /// Invoke <paramref name="func"/> by generic value and return the value itself.
        /// </summary>
        /// <param name="func">The function to invoke.</param>
        /// <returns>The generic value itself.</returns>
        public T Also(Action<T> func)
        {
            func(generic);
            return generic;
        }
    }
}
