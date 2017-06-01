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

		/// This could be made smarter to reduce the number of threads instead of disabling multithreading
		public void SortMultiThread(int[] arr)
		{
			// Arbitrarily-selected threshold
			const int MULTITHREAD_THRESHOLD = 100;

			int l = 0;
			int r = arr.Length - 1;
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

				// Split the array and assign to threads
				var boundaries = new int[numThreads];
				var tasks = new Task[numThreads];
				r = elementsPerThread - 1;
				for (var i = 0; i < numThreads; i++)
				{
					int L = l;
					int R = (i < numThreads - 1 ? r : arr.Length - 1);

					boundaries[i] = L;

					tasks[i] = Task.Run(() =>Sort(arr, L, R));

					if (i == numThreads - 1) break;

					l += elementsPerThread;
					r += elementsPerThread;
				}

				Task.WaitAll(tasks);

				// Merge (still working this part out)
				tasks = new Task[2];
				tasks[0] = Task.Run(() => Merge(arr, 0, boundaries[1], boundaries[2] - 1));
				tasks[1] = Task.Run(() => Merge(arr, boundaries[2], boundaries[3], arr.Length - 1));
				Task.WaitAll(tasks);
				Merge(arr, 0, boundaries[2], arr.Length - 1);
			}
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
