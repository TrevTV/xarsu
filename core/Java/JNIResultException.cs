#if ANDROID
using System;

namespace xarsu.Java;

public class JNIResultException : Exception
{
    public JNI.Result Result { get; set;}

    public JNIResultException(JNI.Result result) : base($"JNI error occurred: {result}")
    {
        this.Result = result;
    }
}
#endif
