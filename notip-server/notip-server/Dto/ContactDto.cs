﻿namespace notip_server.Dto
{
    public class ContactDto
    {
        public int Id { get; set; }
        public string UserCode { get; set; }
        public string ContactCode { get; set; }
        public DateTime Created { get; set; }

        public UserDto User { get; set; }
        public UserDto UserContact { get; set; }
    }
}
