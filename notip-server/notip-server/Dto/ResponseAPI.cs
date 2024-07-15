
using Newtonsoft.Json;

namespace notip_server.Dto
{
    public class ResponseAPI
    {
        public int Status { get; set; }
        public string Message { get; set; } 

        private object _data;
        public object Data
        {
            get
            {
                return JsonConvert.SerializeObject(_data);
            }
            set
            {
                _data = value;
            }
        }
    }
}
