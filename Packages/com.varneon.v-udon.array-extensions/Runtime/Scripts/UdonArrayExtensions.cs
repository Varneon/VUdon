using System;
using UnityEngine;

namespace Varneon.VUdon.UdonArrayExtensions
{
    /// <summary>
    /// Array extension methods for adding partial feature set from List to Udon
    /// </summary>
    public static class UdonArrayExtensions
    {
        /// <summary>
        /// Adds an object to the end of the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        public static T[] Add<T>(this T[] array, T item)
        {
            int length = array.Length;

            T[] newArray = new T[length + 1];

            array.CopyTo(newArray, 0);

            newArray.SetValue(item, length);

            return newArray;
        }

        /// <summary>
        /// Adds an object to the end of the array while ensuring that duplicates are not added
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        public static T[] AddUnique<T>(this T[] array, T item)
        {
            if(Array.IndexOf(array, item) >= 0) { return array; }

            return array.Add(item);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T[] AddRange<T>(this T[] array, T[] collection)
        {
            int length = array.Length;

            int collectionLength = collection.Length;

            T[] newArray = new T[length + collectionLength];

            array.CopyTo(newArray, 0);

            collection.CopyTo(newArray, length);

            return newArray;
        }

        /// <summary>
        /// Determines whether an element is in the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool Contains<T>(this T[] array, T item)
        {
            return Array.IndexOf(array, item) >= 0;
        }

        /// <summary>
        /// Gets the element type of the array type
        /// </summary>
        /// <remarks>
        /// Type.GetElementType() is not exposed in Udon
        /// </remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetElementTypeUdon(this Type type)
        {
            return Type.GetType(type.FullName.TrimEnd(']', '['));
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] GetRange<T>(this T[] array, int index, int count)
        {
            int length = array.Length;

            index = Mathf.Clamp(index, 0, length);

            count = Mathf.Clamp(count, 0, length - index);

            T[] newArray = new T[count];

            Array.Copy(array, index, newArray, 0, count);

            return newArray;
        }

        /// <summary>
        /// Inserts an element into the array at the specified index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T[] Insert<T>(this T[] array, int index, T item)
        {
            int length = array.Length;

            index = Mathf.Clamp(index, 0, length);

            T[] newArray = new T[length + 1];

            newArray.SetValue(item, index);

            if (index == 0)
            {
                Array.Copy(array, 0, newArray, 1, length);
            }
            else if (index == length)
            {
                Array.Copy(array, 0, newArray, 0, length);
            }
            else
            {
                Array.Copy(array, 0, newArray, 0, index);
                Array.Copy(array, index, newArray, index + 1, length - index);
            }

            return newArray;
        }

        /// <summary>
        /// Inserts the elements of a collection into the array at the specified index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T[] InsertRange<T>(this T[] array, int index, T[] collection)
        {
            int length = array.Length;

            int collectionLength = collection.Length;

            int newLength = length + collectionLength;

            index = Mathf.Clamp(index, 0, length);

            T[] newArray = new T[newLength];

            if (index == 0)
            {
                Array.Copy(array, 0, newArray, collectionLength, length);
                Array.Copy(collection, 0, newArray, 0, collectionLength);
            }
            else if (index == length)
            {
                Array.Copy(array, 0, newArray, 0, length);
                Array.Copy(collection, 0, newArray, index, collectionLength);
            }
            else
            {
                Array.Copy(array, 0, newArray, 0, index);
                Array.Copy(collection, 0, newArray, index, collectionLength);
                Array.Copy(array, index, newArray, index + collectionLength, newLength - index - collectionLength);
            }

            return newArray;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        public static T[] Remove<T>(this T[] array, T item)
        {
            int index = Array.IndexOf(array, item);

            if (index == -1) { return array; }

            return array.RemoveAt(index);
        }

        /// <summary>
        /// Removes the element at the specified index of the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            int length = array.Length;

            if(index >= length || index < 0) { return array; }

            int maxIndex = length - 1;

            T[] newArray = new T[maxIndex];

            if (index == 0)
            {
                Array.Copy(array, 1, newArray, 0, maxIndex);
            }
            else if(index == maxIndex)
            {
                Array.Copy(array, 0, newArray, 0, maxIndex);
            }
            else
            {
                Array.Copy(array, 0, newArray, 0, index);
                Array.Copy(array, index + 1, newArray, index, maxIndex - index);
            }

            return newArray;
        }

        /// <summary>
        /// Resizes the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public static T[] Resize<T>(this T[] array, int newSize)
        {
            if(newSize < 0) { newSize = 0; }

            T[] newArray = new T[newSize];

            Array.Copy(array, 0, newArray, 0, Mathf.Min(newSize, array.Length));

            return newArray;
        }

        /// <summary>
        /// Reverses the order of the elements in the entire array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] Reverse<T>(this T[] array)
        {
            Array.Reverse(array);

            return array;
        }
    }
}
