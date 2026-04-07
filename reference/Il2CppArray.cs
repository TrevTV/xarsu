using System.Collections;

namespace xarsu.Reference;

public unsafe class Il2CppArray : Il2CppObject, ICollection, IEnumerable
{
    protected const int DataOffset = 0x20;

    public int Length => (int)*(ulong*)(Pointer.Value + 0x18);
    public int Count => Length;
    public bool IsSynchronized => false;
    public object SyncRoot => this;

    public Il2CppArray(ObjectPointer ptr) : base(ptr) { }

    public static Il2CppArray New(IntPtr elementClass, int length)
    {
        IntPtr ptr = IL2CPP.il2cpp_array_new(elementClass, (uint)length);
        return new Il2CppArray((ObjectPointer)ptr);
    }

    public static Il2CppArray<T> New<T>(int length, int rank = 1)
    {
        IntPtr clazz = IL2CPP.GetIl2CppClassFromType(typeof(T));
        IntPtr arrayClass = IL2CPP.il2cpp_array_class_get(clazz, (uint)rank);
        IntPtr ptr = IL2CPP.il2cpp_array_new(arrayClass, (uint)length);
        return new Il2CppArray<T>((ObjectPointer)ptr);
    }

    public virtual IntPtr this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
            return *(IntPtr*)(Pointer.Value + DataOffset + index * sizeof(IntPtr));
        }
        set
        {
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
            *(IntPtr*)(Pointer.Value + DataOffset + index * sizeof(IntPtr)) = (IntPtr)value!;
        }
    }

    public void CopyTo(Array array, int index)
    {
        for (int i = 0; i < Length; i++)
            array.SetValue(this[i], index + i);
    }

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
            yield return this[i];
    }
}

public class Il2CppArray<T> : Il2CppArray, IEnumerable<T>
{
    public Il2CppArray(ObjectPointer ptr) : base(ptr) { }

    public static unsafe Il2CppArray<T> FromManaged(T[] source)
    {
        var il2Array = New<T>(source.Length);

        if (source.Length == 0)
            return il2Array;

        void* dest = (void*)(il2Array.Pointer.Value + DataOffset);

        if (typeof(T).IsValueType && !typeof(T).IsGenericType)
        {
            int size = NativeUtilities.GetSizeOfReference<T>();
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            fixed (T* src = source)
                Buffer.MemoryCopy(src, dest, (long)source.Length * size, (long)source.Length * size);
#pragma warning restore CS8500
        }
        else
        {
            for (int i = 0; i < source.Length; i++)
                NativeUtilities.WriteValueAtIndex(il2Array.Pointer.Value + DataOffset, i, source[i]);
        }

        return il2Array;
    }

    public new T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
            return NativeUtilities.ReadValueAtIndex<T>(IntPtr.Add(Pointer.Value, DataOffset), index)!;
        }
        set
        {
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
            NativeUtilities.WriteValueAtIndex(IntPtr.Add(Pointer.Value, DataOffset), index, value);
        }
    }

    public new IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}