using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

public static class ExtensionMethods
{
    public static T ToType<T>(this object obj) where T : class
    {
        return obj as T;
    }

    public static bool IsNull(this object obj)
    {
        return obj == null;
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return (collection == null || !collection.Any());
    }

    public static bool IndexIsNotInRange<T>(this IEnumerable<T> collection, int index)
    {
        if(collection.IsNullOrEmpty()) throw new Exception("ExtensionMethods.IndexIsNotInRange(Collection is null)");
        return (index < 0 || index >= collection.Count());
    }

    public static int GetNextIndex<T>(this IEnumerable<T> collection, int currentIndex)
    {
        if(collection.IsNullOrEmpty()) throw new Exception("ExtensionMethods.GetNextIndex(Collection is null)");

        var next = currentIndex + 1;
        return (currentIndex < 0) || (next >= collection.Count()) ? 0 : next;
    }

    public static T RemoveRandomItem<T>(this List<T> list)
    {
        if(list.IsNullOrEmpty())
        {
            return default(T);
        }

        var randomIndex = Random.Range(0, list.Count);
        var item = list[randomIndex];
        list.Remove(item);
        return item;
    }

    public static T RemoveRandomItemElse<T>(this List<T> list, params T[] array)
    {
        if(list.IsNullOrEmpty())
        {
            return default(T);
        }

        var randomIndex = 0;
        T item = default(T);

        if(list.Count == 1 && array.Contains(list[0]))
        {
            item = list[0];
        }
        else do 
        {
            randomIndex = Random.Range(0, list.Count);
            item = list[randomIndex];
        } 
        while(array.Contains(item)); 
        
        list.Remove(item);
        return item;
    }

    public static T GetRandomItem<T>(this IEnumerable<T> collection)
    {
        if(collection.IsNullOrEmpty())
        {
            return default(T);
        }

        var randomIndex = Random.Range(0, collection.Count());
        var item = collection.ElementAt(randomIndex);
        return item;
    }

    public static Enum GetRandomItem(this Enum myEnum)
    {
        var array = Enum.GetValues(myEnum.GetType());
        var index = Random.Range(0,array.Length);
        return (Enum)(array.GetValue(index));
    }
}

