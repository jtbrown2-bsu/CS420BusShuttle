using Core.Models;

namespace View.Models
{
    public class EntryViewModel
    {
        public int BusId { get; set; }
        public int LoopId { get; set; }
        public int StopId { get; set; }
        public int Boarded { get; set; }
        public int LeftBehind { get; set; }
    }
}
