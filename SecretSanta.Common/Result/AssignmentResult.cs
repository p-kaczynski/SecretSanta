using System.Collections.Generic;
using SecretSanta.Domain.Models;

namespace SecretSanta.Common.Result
{
    public class AssignmentResult : ResultBase
    {
        public ICollection<Abandoned> Abandoned { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public Dictionary<long, SantaUser> UserDisplayById { get; set; }
    }
}