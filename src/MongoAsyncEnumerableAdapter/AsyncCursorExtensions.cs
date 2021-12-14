using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;

namespace MongoAsyncEnumerableAdapter
{
    /// <summary>
    /// Provides extension methods for <see cref="IAsyncCursor{T}" />.
    /// </summary>
    public static class AsyncCursorExtensions
    {
        /// <summary>
        /// Wraps a cursor in an <see cref="IAsyncEnumerable{T}" /> that can be enumerated one time.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <param name="cursor">The cursor.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}" /></returns>
        public static IAsyncEnumerable<TDocument> ToAsyncEnumerable<TDocument>(this IAsyncCursor<TDocument> cursor)
        {
            if (cursor is null) throw new ArgumentNullException(nameof(cursor));

            return new AsyncCursorAsyncEnumerableOneTimeAdapter<TDocument>(cursor);
        }

        internal static async IAsyncEnumerable<TDocument> ToAsyncEnumerable<TDocument>(
            IAsyncCursorSource<TDocument>? source,
            IAsyncCursor<TDocument>? cursor,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Debug.Assert(source is null ^ cursor is null);

            cursor ??= await source!.ToCursorAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    foreach (var document in cursor.Current)
                    {
                        yield return document;
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
            }
            finally
            {
                if (cursor is AsyncCursor<TDocument> asyncCursor)
                {
                    await asyncCursor.CloseAsync(cancellationToken).ConfigureAwait(false);
                }

                cursor.Dispose();
            }
        }

        private class AsyncCursorAsyncEnumerableOneTimeAdapter<TDocument> : IAsyncEnumerable<TDocument>
        {
            private readonly IAsyncCursor<TDocument> _cursor;

            private bool _enumerated;

            public AsyncCursorAsyncEnumerableOneTimeAdapter(IAsyncCursor<TDocument> cursor)
            {
                _cursor = cursor;
            }

            public IAsyncEnumerator<TDocument> GetAsyncEnumerator(CancellationToken cancellationToken)
            {
                if (_enumerated)
                {
                    throw new InvalidOperationException("An IAsyncCursor can only be enumerated once.");
                }

                _enumerated = true;

                return 
                    ToAsyncEnumerable(source: null, _cursor, cancellationToken: default).
                    GetAsyncEnumerator(cancellationToken);
            }
        }
    }
}