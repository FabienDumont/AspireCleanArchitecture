namespace AspireCleanArchitecture.Application.InternalUtilities;

/// <summary>
/// Provides extension methods for mapping operations on objects and collections.
/// </summary>
public static class MapperExtensions
{
  #region Methods

  /// <summary>
  /// Applies a mapping function to the input object and returns the mapped result.
  /// </summary>
  /// <typeparam name="TInput">The type of the input object.</typeparam>
  /// <typeparam name="TResult">The type of the result object.</typeparam>
  /// <param name="input">The input object to be mapped.</param>
  /// <param name="mapper">A function that defines the mapping logic from TInput to TResult.</param>
  /// <returns>The mapped result of type TResult.</returns>
  public static TResult Map<TInput, TResult>(this TInput input, Func<TInput, TResult> mapper)
  {
    return mapper(input);
  }

  /// <summary>
  /// Maps a collection of input items to a distinct list of transformed results using the provided mapping function.
  /// </summary>
  /// <typeparam name="TInput">The type of the input items in the collection.</typeparam>
  /// <typeparam name="TResult">The type of the output items after transformation.</typeparam>
  /// <param name="input">The collection of input items to be mapped.</param>
  /// <param name="mapper">The function used to transform each input item into the desired output type.</param>
  /// <returns>A distinct list of transformed items of type <typeparamref name="TResult"/>.</returns>
  public static List<TResult> MapCollection<TInput, TResult>(
    this IEnumerable<TInput> input, Func<TInput, TResult> mapper
  )
  {
    return input.Select(mapper).Distinct().ToList();
  }

  #endregion
}
