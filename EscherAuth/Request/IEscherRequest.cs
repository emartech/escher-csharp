namespace EscherAuth.Request
{
    public interface IEscherRequest
    {
        string Method { get; set; }
        string Url { get; set; }
        Header[] Headers { get; set; }
        string Body { get; set; }
    }
}
