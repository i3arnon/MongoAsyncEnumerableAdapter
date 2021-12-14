using System;
using System.Collections.Generic;
using MongoDB.Driver.Linq;

namespace MongoAsyncEnumerableAdapter
{
    /// <summary>
    /// Provides extension methods for <see cref="IMongoQueryable{T}" />.
    /// </summary>
    public static class MongoQueryableExtensions
    {
        /// <summary>
        /// Wraps an <see cref="IMongoQueryable{T}" /> in an <see cref="IAsyncEnumerable{T}" />. Each time <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator" /> is called a new cursor is fetched from the cursor source.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}" />.</returns>
        public static IAsyncEnumerable<TDocument> ToAsyncEnumerable<TDocument>(
            this IMongoQueryable<TDocument> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            return AsyncCursorSourceExtensions.ToAsyncEnumerable(source);
        }
    }
}