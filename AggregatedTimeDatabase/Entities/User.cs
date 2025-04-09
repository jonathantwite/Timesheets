﻿namespace AggregatedTimeDatabase.Entities;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public DateTime LastEndTime { get; set; }

    public ICollection<JobTotal> JobTotals = [];
}
