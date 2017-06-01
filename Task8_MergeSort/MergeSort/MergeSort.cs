using System.Collections.Generic;
using System.Threading.Tasks;

namespace MergeSort
{
	/// <summary>
	/// This is a port of the Java implementation at http://www.geeksforgeeks.org/merge-sort/, with the original Sort method
	/// wrapped with new methods for single- and multi-threaded operation.
	/// </summary>
	public class MergeSort
	{
		public void SortSingleThread(int[] arr)
		{
			Sort(arr, 0, arr.Length - 1);
		}

		// This could be made smarter to optimize the number of threads instead of disabling multithreading.
		// I'm sure the implementation could be more elegant, too.
		public void SortMultiThread(int[] arr)
		{
			// Arbitrarily-selected threshold
			const int MULTITHREAD_THRESHOLD = 10;

			int l = 0;
			int r = arr.Length - 1;

			// Must be an even number, and should be a method parameter.  I'll keep it
			// simple and try to avoid gold-plating this test.
			int numThreads = 4;

			if (l < r)
			{
				var elementsPerThread = arr.Length / numThreads;

				// Don't incur overhead of multiple threads for small arrays
				if (elementsPerThread < MULTITHREAD_THRESHOLD)
				{
					SortSingleThread(arr);
					return;
				}
				else
				{
					var divisions = Divide(0, arr.Length, numThreads);
					var tasks = new Task[numThreads];

					for (var i = 0; i <= divisions.Length; i++)
					{
						int L = i == 0 ? 0 : divisions[i - 1];
						int R = i < divisions.Length ? divisions[i] - 1 : arr.Length - 1;
						tasks[i] = Task.Run(() => Sort(arr, L, R));
					}

					Task.WaitAll(tasks);

					// Merge
					tasks = new Task[2];
					tasks[0] = Task.Run(() => Merge(arr, 0, divisions[0] - 1, divisions[1] - 1));
					tasks[1] = Task.Run(() => Merge(arr, divisions[1], divisions[2] - 1, arr.Length - 1));
					Task.WaitAll(tasks);
					Merge(arr, 0, divisions[1] - 1, arr.Length - 1);
				}
			}
		}

		public int[] Divide(int l, int r, int segments, bool sort = true)
		{
			var len = r - l + 1;

			if (len < segments) return new int[0];

			var indexes = new List<int>();
			do
			{
				int m = (l + r) / 2;
				indexes.Add(m);

				if (indexes.Count >= segments - 1) break;

				indexes.AddRange(Divide(l, m, segments / 2, false));
				indexes.AddRange(Divide(m + 1, r, segments / 2, false));

			} while (indexes.Count < segments - 1);

			if (sort)
			{
				indexes.Sort();
			}

			return indexes.ToArray();
		}

		private void Sort(int[] arr, int l, int r)
		{
			if (l < r)
			{
				// Find the middle point
				int m = (l + r) / 2;

				// Sort first and second halves
				Sort(arr, l, m);
				Sort(arr, m + 1, r);
				
				// Merge the sorted halves
				Merge(arr, l, m, r);
			}
		}

		private void Merge(int[] arr, int l, int m, int r)
		{
			int i = 0, j = 0;

			// Find sizes of two subarrays to be merged
			int n1 = m - l + 1;
			int n2 = r - m;

			// Create temp arrays
			int[] L = new int[n1];
			int[] R = new int[n2];

			// Copy data to temp arrays
			for (i = 0; i < n1; ++i)
			{
				L[i] = arr[l + i];
			}
			for (j = 0; j < n2; ++j)
			{
				R[j] = arr[m + 1 + j];
			}


			/* Merge the temp arrays */

			// Initial indexes of first and second subarrays
			i = 0;
			j = 0;

			// Initial index of merged subarry array
			int k = l;
			while (i < n1 && j < n2)
			{
				if (L[i] <= R[j])
				{
					arr[k] = L[i];
					i++;
				}
				else
				{
					arr[k] = R[j];
					j++;
				}
				k++;
			}

			// Copy remaining elements of L[] if any
			while (i < n1)
			{
				arr[k] = L[i];
				i++;
				k++;
			}

			// Copy remaining elements of R[] if any
			while (j < n2)
			{
				arr[k] = R[j];
				j++;
				k++;
			}
		}

	}
}
