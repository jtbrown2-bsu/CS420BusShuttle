﻿namespace Core.Models;

public class Loop
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Route> Routes { get; set; } = new List<Route>();
}
