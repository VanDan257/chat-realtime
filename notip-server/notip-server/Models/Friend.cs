namespace notip_server.Models
{
    public class Friend
    {
        #region property
        public int Id { get; set; }
        public Guid SenderCode { get; set; }
        public Guid ReceiverCode { get; set; }
        public string Status { get; set; }

        #endregion

    }
}
