namespace DepVis.Shared.Model
{
    public class CWE
    {
        public long Id { get; set; }

        public ICollection<Vulnerability> Vulnerabilities{ get; set; } = [];
    }
}
