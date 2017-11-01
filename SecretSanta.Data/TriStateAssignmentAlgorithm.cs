using System;
using System.Collections.Generic;
using System.Linq;
using SecretSanta.Common.Result;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;

namespace SecretSanta.Data
{
    public class TriStateAssignmentAlgorithm : AssignmentAlgorithm
    {
        private static readonly Random Random = new Random();
        public override AssignmentResult Assign(ICollection<SantaUser> users)
        {
            // prepare list for abandoned users
            var abandoned = new List<Abandoned>();

            // 1. Remove people who cannot be assigned
            //    That includes:
            //    - alone in the country and won't send abroad
            var loners = new HashSet<long>(users.GroupBy(user => user.Country)
                .Where(g => g.Count() == 1 && g.Single().SendAbroad == SendAbroadOption.WillNot)
                .SelectMany(g => g).Select(user => user.Id));

            abandoned.AddRange(loners.Select(userId=>new Abandoned{SantaUserId = userId, Reason = AbandonmentReason.LoneWontSend}));

            var usersThatCanBeMatched = users.Where(user => !loners.Contains(user.Id)).ToList() as IList<SantaUser>;
            // 2. We should be able to match rest, save for some weird mathematical issues

            // let's try a following concept:
            //      1. Randomize people list
            //      2. Pick first
            //      3. Find a person that will continue the chain who:
            //      3.1 Current person prefers to send to
            //      3.2 If none, person they CAN send to
            //      3.3 If none, but chain length > 1, see if they can send to first pirson in chain. If so, close chain and start again
            //      3.4 If no, remove person, try with different
            //      4. Continue chain (goto 3 current person = end of list)

            var closedChains = new List<LinkedList<SantaUser>>();

            var linkedList = new LinkedList<SantaUser>(usersThatCanBeMatched.OrderBy(_ => Random.Next()));
            while (linkedList.Count > 1) // we need at least two to make a loop
            {
                // create a linked list by randomizing our users
                var firstUser = linkedList.First.Value;
                var chain = CreateChain(linkedList);

                if (chain == null)
                {
                    // ok, if we got no chain, it means that the list was probably recombined
                    // we can try again... but only if we are not repeating ourselves
                    // we will surely repeat ourselves if the first user of the list is the same
                    if (linkedList.First.Value.Id == firstUser.Id)
                    {
                        // we seem to have tried a number of permutations, let's leave - nothing can be done, really
                        break;
                    }
                    // we got null, but we will try again with different starting user
                }
                else
                {
                    // we got a chain, great!
                    closedChains.Add(chain);
                    // and the linked list has been updated by CreateChain
                }
            }

            // ok, we have either ended up with a list of 1 person, who was unfortunate enough to not get matched, or we ended up with a list of folk who we cannot match no matter what we try
            // this is not great - I have a feeling that a different random order when initially creating linked list might have given different results
            // which means, that the algorithm is not providing optimal solution, but "a" solution.
            // this will be tested - I don't feel like actually proving a theorem on that...

            // so, whoever is left on the list is abandoned
            abandoned.AddRange(linkedList.Select(user => new Abandoned{SantaUserId = user.Id, Reason = AbandonmentReason.ComputerSaysNo}));

            return new AssignmentResult
            {
                Abandoned = abandoned,
                Assignments = closedChains.SelectMany(AssignFromChain).ToArray(),
                Success = closedChains.Any()
            };
            
        }

        private static IEnumerable<Assignment> AssignFromChain(LinkedList<SantaUser> chain)
        {
            var sender = chain.First;
            while (sender.Next != null)
            {
                yield return new Assignment{GiverId = sender.Value.Id, RecepientId = sender.Next.Value.Id};
                sender = sender.Next;
            }
            // no next, so loop around
            yield return new Assignment{GiverId = sender.Value.Id, RecepientId = chain.First.Value.Id};
        }

        /// <summary>
        /// Attempts to create a chain from the users in the list.
        /// </summary>
        /// <param name="list">linked list of users</param>
        /// <returns></returns>
        private static LinkedList<SantaUser> CreateChain(LinkedList<SantaUser> list)
        {
            // Start a new chain with first person
            var chain = new LinkedList<SantaUser>();
            var rejects = new List<SantaUser>();

            // now we have:
            // chain: an empty linked list 
            // list: all available users
            // rejects: empty list

            while (list.Count > 0)
            {
                if (chain.Count == 0)
                {
                    if (list.Count == 1)
                    {
                        // this basically means that we failed.
                        // here:
                        // chain is empty
                        // list has one person
                        // rejects has at least one person

                        // assert whether the algorithm logic is correct:
                        if(!rejects.Any())
                            throw new InvalidOperationException($"{nameof(TriStateAssignmentAlgorithm)} is buggy - reached a statement that should never be executed.");

                        // only thing we can do is admit defeat
                        // return reject to list
                        foreach (var reject in rejects)
                            list.AddLast(reject);

                        // and return no chain (it's empty)
                        return null;
                    }

                    chain.AddLast(list.First.Value);
                    list.RemoveFirst();
                    // now we have:
                    // chain: a linked list initialized with a user
                    // list: all remaining users
                }
                // here we have:
                // chain: a linked list with at least 1 user
                // list: a linked list with at least 1 user

                var target = FindRecipient(chain.Last.Value, list, PrefersToSend);
                if (target == null)
                {
                    // didn't find prefered target, look for POSSIBLE targets
                    target = FindRecipient(chain.Last.Value, list, CanSend);
                    if (target == null)
                    {
                        // can we at least close a chain here?
                        if (chain.Last.Value.Id != chain.First.Value.Id && CanSend(chain.Last.Value, chain.First.Value))
                        {
                            //  Yes we can - the last person is NOT the first person in the chain AND last person CAN send to first
                            // the last person would have been already removed from the list, so we... can just return the chain
                            return chain;
                        }
                        // nope, the chain cannot be continued, it cannot be closed
                        // what we can do, is remove last person and try again (it might empty the chain, but we have a "add first from list to chain" clause at the begining of the while)

                        // save reject to the list
                        rejects.Add(chain.Last.Value);
                        // remove reject from chain
                        chain.RemoveLast();
                        // restart iteration
                        continue;
                    }
                }
                // either we got a target, returned chain or restarted the loop
                // therefore here target IS NOT null

                // we ensured that the last person in the chain at least CAN send to target, so add to chain
                chain.AddLast(target.Value);
                // and remove from available list:
                list.Remove(target);
            }
            
            // we have finished with the list
            // add the rejects to the list
            foreach (var user in rejects)
                list.AddLast(user);

            // now, final check: did we manage to create a chain? Maybe we ran out of people in the list?
            if (chain.Last.Value.Id != chain.First.Value.Id && CanSend(chain.Last.Value, chain.First.Value))
            {
                // yes! it's a chain of at least two users that can loop! we can return safely
                return chain;
            }
            
            // TODO We can merge this ^ with this v using do/while - not doing now because time, and it's a bit more readable like that really

            // oops! Our chain is faulty!
            // what we can do now, is return the last links into the original list until we can return a looping chain
            while (chain.Count > 1)
            {
                // add last to list
                list.AddLast(chain.Last.Value);
                // and remove from chain
                chain.RemoveLast();

                // can we loop now?
                if (chain.Last.Value.Id != chain.First.Value.Id && CanSend(chain.Last.Value, chain.First.Value))
                    return chain; // yay, success

                // no, try again
            }
            // well, shit - the chain.Count == 1, it's not a looping chain then
            // return the person to the list and hope for next iteration
            list.AddLast(chain.Single()); // use single to make sure the algorithm is correct
            return null; // we failed.
        }

        private static LinkedListNode<SantaUser> FindRecipient(SantaUser sender, LinkedList<SantaUser> list, Func<SantaUser, SantaUser, bool> predicate)
        {
            var target = list.First;
            while(target != null){ 
                if (predicate(sender, target.Value))
                    return target;
                target = target.Next;
            }
            return null;
        }

        /// <summary>
        /// Checks if the target is user's prefered target.
        /// This means: either the person doesn't care about country of destination, or their countries match
        /// </summary>
        private static bool PrefersToSend(SantaUser gifter, SantaUser recipient) =>
            gifter.SendAbroad == SendAbroadOption.Want || gifter.Country == recipient.Country;

        private static bool CanSend(SantaUser gifter, SantaUser recipient)
            => gifter.SendAbroad == SendAbroadOption.Can || gifter.Country == recipient.Country;
    }
}