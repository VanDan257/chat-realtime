namespace notip_server.ViewModel.ChatBoard
{
    public class CountMessageByMonthResponse
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int NumberOfMessage { get; set; }
        public int NumberOfUserLogin { get; set; }
    }
}
