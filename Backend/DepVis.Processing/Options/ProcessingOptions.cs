namespace DepVis.SbomProcessing.Options;

public class ProcessingOptions
{
    public int WorkerCount { get; set; } = Environment.ProcessorCount;
}

