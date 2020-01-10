// 阶段练习：快速排序

// 简介：
// 从命令行输入10个数字，对其进行快速排序，并将结果输出到屏幕。

// 具体要求：
// 1、 建立一个控制台应用程序。
// 2、 要求用户用命令行方式输入10个数字。如果用户的输入不满足要求，要进行异常处理。
// 3、 用户输入的数字要保存在List<int>中。
// 4、 对List<int>进行快速排序。不能使用.NET内置的排序算法，要求自己写排序的逻辑。
// 5、 将排序后的List<int>中的数据输出到屏幕。

// 考查目的：
// 1、 C#基本语法。
// 2、 常用的类型。

using System;
using System.Collections.Generic;
using System.Text;

namespace Sort
{
  class QuickSort
  {
    const Char SEPARATOR = ' ';
    static List<int> parseInput(string userInput)
    {
      String[] numStrings = userInput.Split(SEPARATOR);
      if (numStrings.Length != 10)
      {
        throw new Exception("Must input 10 numbers.");
      }

      List<int> numbers = new List<int>();
      Array.ForEach(numStrings, num => numbers.Add(Convert.ToInt32(num)));

      return numbers;
    }
    public delegate int compare(int a, int b);
    static int decCompare(int a, int b)
    {
      return b - a;
    }
    static int partition(List<int> list, compare fn, int left, int right)
    {
      var baseNum = list[left];
      while (left < right)
      {
        while (left < right && fn(baseNum, list[right]) <= 0) right--;
        list[left] = list[right];
        while (left < right && fn(list[left], baseNum) <= 0) left++;
        list[right] = list[left];
      }
      // leftIdx equals rightIdx
      list[left] = baseNum;
      return left;
    }
    static void sort(List<int> list, compare fn = null, int? left = null, int? right = null)
    {
      var compareFunction = fn ?? decCompare;
      var initLeft = left ?? 0;
      var initRight = right ?? list.Count - 1;

      if (initLeft >= initRight) return; // done

      int idx = partition(list, compareFunction, initLeft, initRight);

      sort(list, compareFunction, initLeft, idx - 1);
      sort(list, compareFunction, idx + 1, initRight);
    }
    static void Main(string[] args)
    {
      Boolean isInputInvalid = true;
      while (isInputInvalid)
      {
        try
        {
          Console.WriteLine("Please input 10 integers, separate with 'Space': ");
          string userInput = Console.ReadLine();
          List<int> numbers = parseInput(userInput);

          sort(numbers);

          StringBuilder outputStr = new StringBuilder("After Sort: ");
          numbers.ForEach(num => outputStr.Append($"{num} "));
          Console.WriteLine(outputStr);

          isInputInvalid = false;
        }
        catch (Exception ex)
        {
          Console.WriteLine("Error: {0}", ex.Message);
        }
      }
    }
  }
}