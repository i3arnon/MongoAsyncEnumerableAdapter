using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace MongoAsyncEnumerableAdapter
{
    /// <summary>
    /// Provides extension methods for <see cref="IAsyncCursorSource{T}" />.
    /// </summary>
    public static class AsyncCursorSourceExtensions
    {
        /// <summary>
        /// Wraps a cursor source in an <see cref="IAsyncEnumerable{T}" />. Each time <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator" /> is called a new cursor is fetched from the cursor source.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}" />.</returns>
        public static IAsyncEnumerable<TDocument> ToAsyncEnumerable<TDocument>(
            this IAsyncCursorSource<TDocument> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            return AsyncCursorExtensions.ToAsyncEnumerable(source, cursor: null);
        }
    }
}