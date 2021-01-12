using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Naspey.SlickPainter
{
    public static class UtilityExtensions
    {
        /// <summary>
        /// Deletes all unity objects in the enumerable from the memory.
        /// </summary>
        public static void DestroyAll(this IEnumerable<Object> unityObjects)
        {
            foreach (var unityObject in unityObjects)
                Object.Destroy(unityObject);
        }

        /// <summary>
        /// Deletes Unity textures from the IList at provided index.
        /// </summary>
        public static void DestroyAndRemove(this IList<Texture2D> list, int index)
        {
            if (list.Count <= index)
                return;

            Object.Destroy(list[index]);
            list.RemoveAt(index);
        }

        public static void DestroyAndRemoveRange(this List<Texture2D> list, int index, int count)
        {
            for (int i = index; i < index + count; i++)
            {
                if (i < list.Count)
                    Object.Destroy(list[i]);
            }

            if (index < list.Count)
                list.RemoveRange(index, Mathf.Min(count, list.Count - index));
        }
    }
}