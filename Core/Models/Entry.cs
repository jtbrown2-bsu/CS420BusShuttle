namespace Core.Models;

public class Entry
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public int Boarded { get; set; }
    public int LeftBehind { get; set; }

    public string DriverId { get; set; }
    public virtual Driver Driver { get; set; }

    public int BusId { get; set; }
    public virtual Bus Bus { get; set; }

    public int LoopId { get; set; }
    public virtual Loop Loop { get; set; }

    public int RouteId { get; set; }
    public virtual Route Route { get; set; }

    public int StopId { get; set; }
    public virtual Stop Stop { get; set; }

}