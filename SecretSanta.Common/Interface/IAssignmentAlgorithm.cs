using System.Collections;
using System.Collections.Generic;
using SecretSanta.Common.Result;
using SecretSanta.Domain.Models;

namespace SecretSanta.Common.Interface
{
    public interface IAssignmentAlgorithm
    {
        AssignmentResult Assign(ICollection<SantaUser> users);
        void Verify(AssignmentResult result);
    }
}