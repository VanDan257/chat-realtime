namespace notip_server.ViewModel.ChatBoard
{
    public class UpdateGroupAvatarRequest
    {
        public Guid Code { get; set; }
        public List<IFormFile> Image{ get; set; }
    }
}
