namespace Tasks4U.Models.ViewModels
{
    public class PagingInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SelectedDate { get; set; }
        public bool MyTasks { get; set; }
        public bool CompletedTasks { get; set; }

    }
}
