using System.Collections.Generic;
using System.Linq;
using SecretSanta.Common.Interface;
using SecretSanta.Common.Result;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;

namespace SecretSanta.Data
{
    public class TriStateAssignmentAlgorithm : IAssignmentAlgorithm
    {
        public AssignmentResult Assign(ICollection<SantaUser> users)
        {
            var abandoned = new List<Abandoned>();
            // 1. Remove people who cannot be assigned
            //    That includes:
            //    - alone in the country and won't send abroad
            var loners = new HashSet<long>(users.GroupBy(user => user.Country)
                .Where(g => g.Count() == 1 && g.Single().SendAbroad == SendAbroadOption.WillNot)
                .SelectMany(g => g).Select(user => user.Id));

            abandoned.AddRange(loners.Select(userId=>new Abandoned{SantaUserId = userId, Reason = "Alone in the country and will not send abroad"}));

            var usersThatCanBeMatched = users.Where(user => !loners.Contains(user.Id)).ToList();

            // 2. We should be able to match rest, save for some weird mathematical issues
        }
    }
}