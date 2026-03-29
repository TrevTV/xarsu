namespace xarsu.Reference;

// TODO: extend this, implement it properly into Il2CppSystem.Arrays, generics, etc
public unsafe class Il2CppObjectArray : Il2CppObject
{
    private const int DataOffset = 0x20;

    public Il2CppObjectArray(ObjectPointer ptr) : base(ptr) { }

    public static Il2CppObjectArray New(IntPtr elementClass, int length)
    {
        IntPtr ptr = IL2CPP.il2cpp_array_new(elementClass, (uint)length);
        return new((ObjectPointer)ptr);
    }

    public int Length => (int)*(ulong*)(Pointer.Value + 0x18);

    public IntPtr this[int index]
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
            *(IntPtr*)(Pointer.Value + DataOffset + index * sizeof(IntPtr)) = value;
        }
    }
}