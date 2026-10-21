#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Balltallion.Editor
{
    /// <summary>
    /// Editor-only. Our own helper (no plugin needed) that makes sure no two [SerializeReference]
    /// slots point at the same object.
    ///                                     MADE BY CLAUDE
    /// </summary>
    public static class SharedReferenceFixer
    {
        private const int MaxDepth = 32;
        private const BindingFlags InstanceFields =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        private static readonly Dictionary<Type, FieldInfo[]> ReferenceFieldsByType = new();
        private static readonly Dictionary<Type, FieldInfo[]> ByValueFieldsByType = new();

        /// <summary>
        /// Finds objects that sit in more than one [SerializeReference] slot of <paramref name="host"/>
        /// and gives every later slot its own copy. Looks through nested [SerializeReference] objects and
        /// through ordinary serialized classes, structs and lists of them. Returns true if anything changed.
        /// </summary>
        public static bool SeparateSharedReferences(object host)
        {
            if (host == null) return false;
            HashSet<object> seen = new HashSet<object>(ReferenceComparer.Instance);
            return Visit(host, seen, 0);
        }

        private static bool Visit(object owner, HashSet<object> seen, int depth)
        {
            if (depth > MaxDepth) return false;
            Type type = owner.GetType();
            bool changed = false;

            // [SerializeReference] slots: each object may appear only once.
            foreach (FieldInfo field in GetFields(type, ReferenceFieldsByType, IsReferenceField))
            {
                object value = field.GetValue(owner);
                if (value == null) continue;

                if (value is IList list) // List<T> or T[]
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        object item = list[i];
                        if (item == null) continue;
                        if (!seen.Add(item))
                        {
                            item = CopyOf(item);
                            list[i] = item;
                            seen.Add(item);
                            changed = true;
                        }
                        changed |= Visit(item, seen, depth + 1);
                    }
                }
                else
                {
                    if (!seen.Add(value))
                    {
                        value = CopyOf(value);
                        field.SetValue(owner, value);
                        seen.Add(value);
                        changed = true;
                    }
                    changed |= Visit(value, seen, depth + 1);
                }
            }

            // Ordinary serialized classes and structs (and lists of them) are copied by value when a list
            // element is duplicated, so a [SerializeReference] field inside one can still end up shared.
            // Look inside them, but don't count them as shared themselves.
            foreach (FieldInfo field in GetFields(type, ByValueFieldsByType, IsByValueContainerField))
            {
                object value = field.GetValue(owner);
                if (value == null) continue;

                if (value is IList list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        object item = list[i];
                        if (item == null || !Visit(item, seen, depth + 1)) continue;
                        changed = true;
                        if (item.GetType().IsValueType) list[i] = item; // write the edited struct back
                    }
                }
                else if (Visit(value, seen, depth + 1))
                {
                    changed = true;
                    if (value.GetType().IsValueType) field.SetValue(owner, value); // write the edited struct back
                }
            }

            return changed;
        }

        /// <summary>A new object of the same type with the same saved values.</summary>
        private static object CopyOf(object source)
        {
            object copy = Activator.CreateInstance(source.GetType(), true);
            try
            {
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(source), copy);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Couldn't copy the values of a shared {source.GetType().Name}, so the new one starts with defaults. {exception.Message}");
            }
            return copy;
        }

        // ---------- Which fields to look at ----------

        private static FieldInfo[] GetFields(Type type, Dictionary<Type, FieldInfo[]> cache, Func<FieldInfo, bool> include)
        {
            if (cache.TryGetValue(type, out FieldInfo[] cached)) return cached;

            List<FieldInfo> fields = new List<FieldInfo>();
            for (Type t = type; t != null && t != typeof(object) && t != typeof(Object); t = t.BaseType)
            {
                foreach (FieldInfo field in t.GetFields(InstanceFields))
                {
                    if (include(field)) fields.Add(field);
                }
            }

            FieldInfo[] result = fields.ToArray();
            cache[type] = result;
            return result;
        }

        private static bool IsReferenceField(FieldInfo field)
        {
            return field.IsDefined(typeof(SerializeReference), false);
        }

        /// <summary>A field Unity saves by value whose type (or element type) could hold [SerializeReference] fields.</summary>
        private static bool IsByValueContainerField(FieldInfo field)
        {
            if (field.IsInitOnly || field.IsDefined(typeof(SerializeReference), false)) return false;
            if (field.IsDefined(typeof(NonSerializedAttribute), false)) return false;
            if (!field.IsPublic && !field.IsDefined(typeof(SerializeField), false)) return false;
            return IsContainerType(ElementTypeOf(field.FieldType));
        }

        private static Type ElementTypeOf(Type type)
        {
            if (type.IsArray) return type.GetElementType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) return type.GetGenericArguments()[0];
            return type;
        }

        private static bool IsContainerType(Type type)
        {
            if (type == null || type.IsPrimitive || type.IsEnum || type == typeof(string)) return false;
            if (typeof(Object).IsAssignableFrom(type)) return false;
            if (!type.IsSerializable) return false;
            string ns = type.Namespace ?? string.Empty;
            return !ns.StartsWith("System", StringComparison.Ordinal) && !ns.StartsWith("Unity", StringComparison.Ordinal);
        }

        private sealed class ReferenceComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceComparer Instance = new();
            bool IEqualityComparer<object>.Equals(object a, object b) => ReferenceEquals(a, b);
            int IEqualityComparer<object>.GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}
#endif
