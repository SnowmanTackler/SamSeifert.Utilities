namespace SamSeifert.Utilities.Files.Json
{
    public interface JsonPackable
    {
        JsonDict Pack();
        void Unpack(JsonDict dict);
    }
}
