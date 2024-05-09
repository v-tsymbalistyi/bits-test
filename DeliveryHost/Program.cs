// See https://aka.ms/new-console-template for more information
using DeliverySystem.Bits.Base;

var manager = new BitsManager();

var jobs = manager.EnumJobs();
if (!jobs.Any())
    Console.WriteLine("No jobs");
else
{
    foreach (var job in manager.EnumJobs())
        Console.WriteLine($"{job.Key}: {job.Value.GetDisplayName()}, owned by {job.Value.GetOwnerName()}");
}

Console.WriteLine("Hello, World!");
