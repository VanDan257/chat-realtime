namespace notip_server.ViewModel.User
{
    public class CreateAdminRequest
    {
        public string UserName {get; set;}
        
        public string Email {get; set;}
       
        public string Gender {get; set;}
       
        public string PhoneNumber {get; set;}
       
        public string Dob {get; set;}
       
        public List<IFormFile> Avatar {get; set;}
       
        public string Role {get; set;}
       
        public string Address {get; set;}
       
        public string Password { get; set; }
    }
}
