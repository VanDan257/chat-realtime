namespace notip_server.Dto
{
    public class BlobDto
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }
    }
}
