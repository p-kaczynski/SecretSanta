using System.Collections.Generic;

namespace SecretSanta.Domain.Models
{
    public class AssignmentResult
    {
        public IList<Abandoned> Abandoned { get; set; }
        public IList<Assignment> Assignments { get; set; }
        public Dictionary<long, SantaUser> UserDisplayById { get; set; }
    }
}