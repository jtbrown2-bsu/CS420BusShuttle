namespace Core.Models;

public class Route
{
    public int Id { get; set; }
    public int Order { get; set; }
    public int StopId { get; set; }
    public virtual Stop Stop { get; set; }
    public int LoopId { get; set; }
    public virtual Loop Loop { get; set; }
}