namespace xarsu.Reference.Java;

public class JString : JObject
{
    public JString() : base() { }

    public string GetString() => JNI.GetJStringString(this);
}