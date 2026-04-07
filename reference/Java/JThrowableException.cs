using System;

namespace xarsu.Reference.Java;

public class JThrowableException : Exception
{
    public JThrowable? Throwable { get; set;}

    public JThrowableException() { }

    public JThrowableException(JThrowable throwable) : base()
    {
        this.Throwable = throwable;
    }
}