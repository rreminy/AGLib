namespace AG.Collections.Concurrent.Buckets
{
    /// <summary>Action executed as part of a Find.</summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="result">Find results.</param>
    /// <param name="entry">
    ///  <list type="bullet">
    ///   <item><see cref="BucketFindResult.NotFound"/>: null ref</item>
    ///   <item><see cref="BucketFindResult.Found"/>, <see cref="BucketFindResult.Removed"/>, <see cref="BucketFindResult.Replaced"/>: Found / old value as ref</item>
    ///   <item><see cref="BucketFindResult.Created"/>: Reference to new value</item>
    ///  </list>
    /// </param>
    internal delegate void BucketFindAction<T>(BucketFindResult result, ref T entry);

    /// <summary>Action executed as part of a Find.</summary>
    /// <typeparam name="TState">Type for <paramref name="state"/>.</typeparam>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="result">Find results.</param>
    /// <param name="state">State to pass.</param>
    /// <param name="entry">
    ///  <list type="bullet">
    ///   <item><see cref="BucketFindResult.NotFound"/>: null ref</item>
    ///   <item><see cref="BucketFindResult.Found"/>, <see cref="BucketFindResult.Removed"/>, <see cref="BucketFindResult.Replaced"/>: Found / old value as ref</item>
    ///   <item><see cref="BucketFindResult.Created"/>: Reference to new value</item>
    ///  </list>
    /// </param>
    internal delegate void BucketFindAction<TState, T>(BucketFindResult result, ref TState? state, ref T entry);
}
