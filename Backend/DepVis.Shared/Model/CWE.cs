namespace DepVis.Shared.Model
{
    public class CWE
    {
        public Guid Id { get; set; }
        public long CweId { get; set; }

        public ICollection<Vulnerability> Vulnerabilities { get; set; } = [];
    }
}
