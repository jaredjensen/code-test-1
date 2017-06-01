using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MergeSortTest
{
	[TestClass]
	public class MergeTests
	{
		MergeSort.MergeSort _mergeSort;

		[TestInitialize]
		public void Init()
		{
			_mergeSort = new MergeSort.MergeSort();
		}

		[TestMethod]
		public void ItShouldDivideEvenLengthsCorrectly()
		{
			var divisions = _mergeSort.Divide(0, 99, 4);
			Assert.AreEqual(3, divisions.Length);
			Assert.AreEqual(24, divisions[0]);
			Assert.AreEqual(49, divisions[1]);
			Assert.AreEqual(74, divisions[2]);
		}

		[TestMethod]
		public void ItShouldDivideUnevenLengthsNearlyLevel()
		{
			var divisions = _mergeSort.Divide(0, 101, 4);
			Assert.AreEqual(3, divisions.Length);
			Assert.AreEqual(25, divisions[0]);
			Assert.AreEqual(50, divisions[1]);
			Assert.AreEqual(76, divisions[2]);
		}

		[TestMethod]
		public void ItShouldSortASingleElementArray()
		{
			var array = new[] { 0 };
			_mergeSort.SortSingleThread(array);
			AssertResultMatchsArraySort(array);
		}

		[TestMethod]
		public void ItShouldSortASmallArray()
		{
			var array = new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 };
			_mergeSort.SortSingleThread(array);
			AssertResultMatchsArraySort(array);
		}

		[TestMethod]
		public void ItShouldSortAnArrayWithEvenNumberOfElements()
		{
			var array = new[] { 4, 3, 2, 1 };
			_mergeSort.SortSingleThread(array);
			AssertResultMatchsArraySort(array);
		}

		[TestMethod]
		public void ItShouldSortAnArrayWithOddNumberOfElements()
		{
			var array = new[] { 5, 4, 3, 2, 1 };
			_mergeSort.SortSingleThread(array);
			AssertResultMatchsArraySort(array);
		}

		[TestMethod]
		public void ItShouldSortALargeRandomArray()
		{
			//var array = GenerateRandomArray(100000, 10000000);
			var array = GenerateRandomArray(50, 50);
			_mergeSort.SortMultiThread(array);
			AssertResultMatchsArraySort(array);
		}

		[TestMethod]
		public void ItShouldSortFasterUsingMultipleThreads()
		{
			const int arraySize = 1000000;

			var array1 = GenerateRandomArray(arraySize, arraySize);
			var array2 = new int[array1.Length];
			Array.Copy(array1, array2, array1.Length);

			var stopwatch = Stopwatch.StartNew();
			_mergeSort.SortSingleThread(array1);
			stopwatch.Stop();
			var singleThreadMs = stopwatch.ElapsedMilliseconds;

			stopwatch = Stopwatch.StartNew();
			_mergeSort.SortMultiThread(array2);
			stopwatch.Stop();
			var multiThreadMs = stopwatch.ElapsedMilliseconds;

			Console.WriteLine($"Single-thread: {singleThreadMs}ms, multi-thread: {multiThreadMs}");
			Console.WriteLine(stopwatch.ElapsedMilliseconds);

			Assert.IsTrue(multiThreadMs < singleThreadMs);
		}

		private void AssertResultMatchsArraySort(int[] result)
		{
			var expected = new int[result.Length];
			Array.Copy(result, expected, result.Length);
			Array.Sort(expected);

			Assert.AreEqual(expected.Length, result.Length, "Arrays are the same size");

			for (var i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], result[i], $"Value at position {i}");
			}
		}

		private int[] GenerateRandomArray(int minLength, int maxLength)
		{
			var random = new Random();
			var length = random.Next(minLength, maxLength);
			var array = new int[length];

			for (var i = 0; i < length; i++)
			{
				array[i] = random.Next();
			}

			return array;
		}
	}
}
