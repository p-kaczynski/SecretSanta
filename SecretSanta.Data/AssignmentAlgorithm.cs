using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using SecretSanta.Common.Interface;
using SecretSanta.Common.Result;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;

namespace SecretSanta.Data
{
    public abstract class AssignmentAlgorithm : IAssignmentAlgorithm
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public abstract AssignmentResult Assign(ICollection<SantaUser> users);

        public void Verify(AssignmentResult result)
        {
            Log.Info($"{nameof(AssignmentAlgorithm)}.{nameof(Verify)}: Success={result.Success}, user count={result.UserDisplayById.Count}, assignments={result.Assignments.Count}, abandoned={result.Abandoned.Count} (Loner:{result.Abandoned.Count(a => a.Reason == AbandonmentReason.LoneWontSend)}, Algorithm: {result.Abandoned.Count(a => a.Reason == AbandonmentReason.ComputerSaysNo)})");

            // 1. All people either matched or saved as abandoned
            if (result.Assignments.Count + result.Abandoned.Count != result.UserDisplayById.Count)
                throw new InvalidOperationException($"{nameof(AssignmentAlgorithm)}.{nameof(Verify)}: Missing people");

            // 2. Nobody should be sending to themselves
            if(result.Assignments.Any(a => a.RecepientId == a.GiverId))
                throw new InvalidOperationException($"{nameof(AssignmentAlgorithm)}.{nameof(Verify)}: Sending to yourself");

            // 3. Nobody should be sending abroad if they did not wish to
            foreach (var a in result.Assignments)
            {
                var giver = result.UserDisplayById[a.GiverId];
                var recipient = result.UserDisplayById[a.RecepientId];
                if (giver.SendAbroad == SendAbroadOption.WillNot && giver.Country != recipient.Country)
                    throw new InvalidOperationException($"{nameof(AssignmentAlgorithm)}.{nameof(Verify)}: Sending abroad against preferences");
            }

            // 4. Everyone who sends, gets
            var givers = new HashSet<long>(result.Assignments.Select(a => a.GiverId));
            var recipients = new HashSet<long>(result.Assignments.Select(a => a.RecepientId));
            if (!givers.SetEquals(recipients))
                throw new InvalidOperationException($"{nameof(AssignmentAlgorithm)}.{nameof(Verify)}: Unequal exchange detected");

            // 5. Duplicates?
            if (result.Assignments.Select(a => a.RecepientId).Distinct().Count() !=
                result.Assignments.Select(a => a.RecepientId).Count())
                throw new InvalidOperationException(
                    $"{nameof(AssignmentAlgorithm)}.{nameof(Verify)}: Duplicates detected");
        }
    }
}