using Core.Models;

namespace View.Models
{
    public class EntryEditViewModel
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public int LoopId { get; set; }
        public int StopId { get; set; }
        public int Boarded { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public int LeftBehind { get; set; }
    }
}
