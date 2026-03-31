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

    public static Il2CppArray<T> New<T>(int length, int rank = 1) where T : Il2CppObject
    {
        IntPtr clazz = IL2CPP.GetIl2CppClassFromType(typeof(T));
        IntPtr arrayClass = IL2CPP.il2cpp_array_class_get(clazz, (uint)rank);
        IntPtr ptr = IL2CPP.il2cpp_array_new(arrayClass, (uint)length);
        return new Il2CppArray<T>((ObjectPointer)ptr);
    }

    public static Il2CppValueArray<T> NewValue<T>(int length, int rank = 1) where T : unmanaged
    {
        IntPtr clazz = IL2CPP.GetIl2CppClassFromType(typeof(T));
        IntPtr arrayClass = IL2CPP.il2cpp_array_class_get(clazz, (uint)rank);
        IntPtr ptr = IL2CPP.il2cpp_array_new(arrayClass, (uint)length);
        return new Il2CppValueArray<T>((ObjectPointer)ptr);
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

public unsafe class Il2CppArray<T> : Il2CppArray, IEnumerable<T> where T : Il2CppObject
{
    public Il2CppArray(ObjectPointer ptr) : base(ptr) { }

    public new T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
            return Wrap<T>(*(IntPtr*)(Pointer.Value + DataOffset + index * sizeof(IntPtr)));
        }
        set
        {
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
            *(IntPtr*)(Pointer.Value + DataOffset + index * sizeof(IntPtr)) = value.Pointer.Value;
        }
    }

    public new IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public unsafe class Il2CppValueArray<T> : Il2CppArray, IEnumerable<T> where T : unmanaged
{
    public Il2CppValueArray(ObjectPointer ptr) : base(ptr) { }

    public new T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
            return *(T*)(Pointer.Value + DataOffset + index * sizeof(T));
        }
        set
        {
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
            *(T*)(Pointer.Value + DataOffset + index * sizeof(T)) = value;
        }
    }

    public new IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}