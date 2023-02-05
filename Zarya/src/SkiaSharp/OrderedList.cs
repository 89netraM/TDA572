using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zarya.SkiaSharp;

internal class OrderedList<TKey, TValue> : IEnumerable<TValue>
	where TKey : notnull
{
	private readonly SortedDictionary<TKey, ICollection<TValue>> sections = new SortedDictionary<TKey, ICollection<TValue>>();

	public void Add(TKey key, TValue value)
	{
		if (!sections.TryGetValue(key, out var section))
		{
			sections.Add(key, section = new List<TValue>());
		}
		section.Add(value);
	}

	public bool Remove(TValue value)
	{
		foreach (var section in sections.Values)
		{
			if (section.Remove(value))
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerator<TValue> GetEnumerator() =>
		sections.Values
			.SelectMany(s => s)
			.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() =>
		GetEnumerator();
}
