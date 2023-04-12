namespace MailerRobot.Bot.Extensions;

public static class LinqExtensions
{
	public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>
	(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outKey,
		Func<TInner, TKey> inKey, Func<TOuter, TInner?, TResult> result)
	{
		return outer.GroupJoin(inner, outKey, inKey,
						(i, o) => new {i, o})
					.SelectMany(e => e.o.DefaultIfEmpty(),
						(o, ot) => result(o.i, ot));
	}

	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
	{
		var forEach = enumerable.ToList();

		foreach (var e in forEach)
			action(e);

		return forEach;
	}

	public static void ForEach<T, TSome>(this IEnumerable<T> enumerable, Func<T, TSome> action)
	{
		foreach (var e in enumerable)
			_ = action(e);
	}

	public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
	{
		foreach (var e in enumerable)
			await action(e);
	}

	public static async IAsyncEnumerable<TResult> SelectAsync<T, TResult>(this IEnumerable<T> enumerable,
		Func<T, Task<TResult>> action)
	{
		foreach (var e in enumerable)
			yield return await action(e);
	}

	public static async IAsyncEnumerable<TResult> SelectAsync<T, TResult>(this IAsyncEnumerable<T> enumerable,
		Func<T, Task<TResult>> action)
	{
		await foreach (var e in enumerable)
			yield return await action(e);
	}

	public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> enumerable,
		Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
	{
		await foreach (var e in enumerable.WithCancellation(cancellationToken))
			await action(e, cancellationToken);
	}

	public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable,
		Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
	{
		foreach (var e in enumerable)
			await action(e, cancellationToken);
	}

	public static async Task ForEachAsync<T>(this Task<List<T>> enumerable, Action<T> action)
	{
		foreach (var e in await enumerable)
			action(e);
	}

	public static async Task ParallelForEachAsync<T>(this IEnumerable<T> enumerable,
		Func<T, CancellationToken, ValueTask> action, int threadCount = 5)
	{
		var parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = threadCount
		};

		await Parallel.ForEachAsync(enumerable, parallelOptions, action);
	}

	public static async Task ForEachAsync<T>(this Task<List<T>> enumerable, Func<T, Task> action)
	{
		foreach (var e in await enumerable)
			await action(e);
	}

	public static async IAsyncEnumerable<TResult> ForEachAsync<T, TResult>(
		this IEnumerable<T> enumerable,
		Func<T, Task<TResult>> action)
	{
		foreach (var e in enumerable)
			yield return await action(e);
	}

	public static async IAsyncEnumerable<TResult> ForEachAsync<T, TResult>(
		this Task<List<T>> enumerable,
		Func<T, Task<TResult>> action)
	{
		foreach (var e in await enumerable)
			yield return await action(e);
	}

	public static async IAsyncEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>
	(this IAsyncEnumerable<TOuter> outer, IEnumerable<TInner> inner,
		Func<TOuter, TKey> outKey,
		Func<TInner, TKey> inKey, Func<TOuter, TInner?, TResult> result)
	{
		var list = new List<TOuter>();

		await foreach (var el in outer)
			list.Add(el);

		var selectMany = list.GroupJoin(inner, outKey, inKey,
								(i, o) => new {i, o})
							.SelectMany(e => e.o.DefaultIfEmpty(),
								(o, ot) => result(o.i, ot));

		foreach (var el in selectMany)
			yield return el;
	}

	public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
	{
		var list = new List<T>();

		await foreach (var el in source)
			list.Add(el);

		return list;
	}
}