namespace SamSeifert.Utilities.Json
{
    public interface JsonPackable
    {
        JsonDict Pack();
        void Unpack(JsonDict dict);
    }
}
