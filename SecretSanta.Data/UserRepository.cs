using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using JetBrains.Annotations;
using SecretSanta.Common.Helpers;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using SecretSanta.Domain.SecurityModels;
using Utilities.Collections.Entities;
using Utilities.Collections.Enumerables;
using SecretSanta.Common.Result;

namespace SecretSanta.Data
{
    public class UserRepository : IUserRepository
    {
        private static readonly Random Random = new Random();
        private readonly IEncryptionProvider _encryptionProvider;
        private readonly string _connectionString;
        public UserRepository(IConfigProvider configProvider, IEncryptionProvider encryptionProvider)
        {
            _encryptionProvider = encryptionProvider;
            _connectionString = configProvider.ConnectionString;
        }

        public long InsertUser([NotNull] SantaUser user)
        {
            _encryptionProvider.Encrypt(user);
            return WithConnection(conn => conn.Insert(user));
        }

        public UserEditResult UpdateUser([NotNull] SantaUser updateUser)
        {
            var emailChanged = !updateUser.Email.Equals(
                WithConnection(conn => conn.Get<SantaUser>(updateUser.Id)).Email, StringComparison.OrdinalIgnoreCase);

            if(emailChanged && !CheckEmail(updateUser.Email))
                    return new UserEditResult{EmailChanged = true, EmailUnavailable = true, Success = false};

            _encryptionProvider.Encrypt(updateUser);

            WithConnection(conn =>
                    conn.Execute($"UPDATE [dbo].[{nameof(SantaUser)}s]   SET " +
                                 "[Email] = @Email " +
                                 "     ,[FacebookProfileUrl] = @FacebookProfileUrl" +
                                 "      ,[DisplayName] = @DisplayName" +
                                 "      ,[FullName] = @FullName" +
                                 "      ,[AddressLine1] = @AddressLine1" +
                                 "      ,[AddressLine2] = @AddressLine2" +
                                 "      ,[PostalCode] = @PostalCode" +
                                 "      ,[City] = @City" +
                                 "      ,[Country] = @Country" +
                                 "      ,[SentAbroad] = @SentAbroad" +
                                 "      ,[Note] = @Note " +
                                 "      ,[AdminConfirmed] = 0 " +
                                 "WHERE [Id] = @Id",
                new
                {
                    updateUser.Id,
                    updateUser.Email,
                    updateUser.FacebookProfileUrl,
                    updateUser.DisplayName,
                    updateUser.FullName,
                    updateUser.AddressLine1,
                    updateUser.AddressLine2,
                    updateUser.PostalCode,
                    updateUser.City,
                    updateUser.Country,
                    updateUser.SentAbroad,
                    updateUser.Note
                }));


            if (emailChanged)
                WithConnection(conn =>
                    conn.Execute($"UPDATE [dbo].[{nameof(SantaUser)}s]   SET [EmailConfirmed] = 0"));

            return new UserEditResult{Success = true, EmailChanged = emailChanged};
        }

        public void SetPassword(PasswordResetModel model) 
            => WithConnection(conn =>
            conn.Execute($"UPDATE {nameof(SantaUser)}s SET PasswordHash = @passwordBytes WHERE Id = @userId",
                new {model.UserId, model.PasswordBytes}));


        public bool CheckEmail(string email) 
            => WithConnection(conn =>conn.QuerySingleOrDefault<SantaUser>(
                                     $"SELECT Email FROM {nameof(SantaUser)}s WHERE Email = @email", new {email})) == null;

        public SantaUser GetUser(long id)
        {
            var model = WithConnection(conn => conn.Get<SantaUser>(id));
            if (model == null)
                return null;

            _encryptionProvider.Decrypt(model);
            return model;
        }

        public SantaUser GetUserWithoutProtectedData(long id)
        {
            var model = WithConnection(conn => conn.Get<SantaUser>(id));
            if (model == null)
                return null;

            model.ClearDataProtected();
            return model;
        }

        public SantaUser GetUserWithoutProtectedDataByEmail([NotNull]string emailAddress)
        {
            var model = WithConnection(conn => conn.QuerySingleOrDefault<SantaUser>($"SELECT * FROM {nameof(SantaUser)}s WHERE Email = @emailAddress", new {emailAddress}));
            if (model == null)
                return null;

            model.ClearDataProtected();
            return model;
        }

        public IList<SantaUser> GetAllUsersWithoutProtectedData()
        {
            var models = WithConnection(conn => conn.GetAll<SantaUser>().ToList());
            foreach(var model in models)
                model.ClearDataProtected();

            return models;
        }

        public void AdminConfirm(long id)
        {
            WithConnection(conn =>
            {
                var model = conn.Get<SantaUser>(id);
                model.AdminConfirmed = true;
                conn.Update(model);
            });
        }

        public void EmailConfirm(long id)
        {
            WithConnection(conn =>
            {
                var model = conn.Get<SantaUser>(id);
                model.EmailConfirmed = true;
                conn.Update(model);
            });
        }

        public void DeleteUser(long id)
        {
            WithConnection(conn =>
            {
                var model = conn.Get<SantaUser>(id);
                conn.Delete(model);
            });
        }

        public bool WasAssigned()
        {
            return WithConnection(conn => conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Assignments") > 0);
        }

        private IEnumerable<Assignment> CreateCircularAssignment(IEnumerable<SantaUser> users, bool randomizeOrder = true)
        {
            if (randomizeOrder)
                users = users.OrderBy(_ => Random.Next());

            var linkedList = new LinkedList<SantaUser>(users);
            if(linkedList.Count < 3)
                throw new InvalidOperationException($"{nameof(CreateCircularAssignment)} is designed to operate on the groups of minimum 3 users, but got {linkedList.Count}!");

            var giver = linkedList.First;
            while (giver.Next != null)
            {
                var target = giver.Next;
                yield return new Assignment{GiverId = giver.Value.Id, RecepientId = target.Value.Id};
                giver = target;
            }
            // giver = last node, wrap around:
            yield return new Assignment{GiverId = giver.Value.Id, RecepientId = linkedList.First.Value.Id};

            // we have just created a circle of people, each gives to the next (random order)
        }


        private IEnumerable<Assignment> CrossMatch(IEnumerable<SantaUser> selectMany)
        {
            var userArray = selectMany.ToArray();
            var wontSent = userArray.Where(user => !user.SentAbroad).ToArray();
            var willSent = userArray.Where(user => !user.SentAbroad).ToArray();

            if(wontSent.Length != willSent.Length)
                throw new InvalidOperationException($"{nameof(CrossMatch)} requires an even number of wills and wonts!");

            return CreateCircularAssignment(wontSent.Intertwine(willSent, false));
        }


        public AssignmentResult AssignRecipients()
        {
            // we will need random
            var random = new Random();

            // prepare predicate
            bool ConfirmedOnly(SantaUser user) => user.AdminConfirmed && user.EmailConfirmed;

            // get all users
            var allUsers = GetAllUsersWithoutProtectedData().Where(ConfirmedOnly).ToList();

            if (!allUsers.Any())
                throw new InvalidOperationException("No users have signed up!");

            // prepare a list of obtained assignments
            var assignments = new List<Assignment>(allUsers.Count);

            // this will be a place for people who sadly cannot be matched
            var abandoned = new List<Abandoned>();

            // plan c, fuck it

            // prepare a helper func:

            var getDonor = new Func<IList<SantaUser>, SantaUser>(userList => userList.GroupBy(user => user.Country)
                .Where(g => g.Count() > 3 && g.Any(user => user.SentAbroad))
                .OrderBy(g => g.Count(user => user.SentAbroad))
                .FirstOrDefault()
                .OrEmpty()
                .Where(user => user.SentAbroad)
                .OrderBy(_ => Random.Next())
                .FirstOrDefault(user => user.SentAbroad));

            // we know who will not make it: folk who are all alone in their country, or have only one other person in, but none of them will send abroad.
            // let's remove them first, this will simplify the following logic
            foreach (var wontMatch in allUsers.GroupBy(user => user.Country)
                .Where(g => g.Count() < 3 && g.All(user => !user.SentAbroad)).SelectMany(g=>g))
            {
                // countries with 1 or 2  users, none of them will send abroad
                abandoned.Add(new Abandoned{SantaUserId = wontMatch.Id,Reason = "One or two people in the country, both wont send abroad."});
                allUsers.Remove(wontMatch);
            }


            // So, every country with at least 3 people we can transfrom into a ring no-problem.
            // The only issue are two-people countries with setup: {WILL, WONT}
            
            // if there is mroe than one of such countries, there is no problem - cross match them, circularly
            // if there is a single country, we need to borrow a person from a surplus country

            // a surplus is when there is >3 users in a country, at least one WILL.
            // in other words, if the country has >3 users, and Any(user=>user.SentAbroad) we have Count - Count(user=>!user.SentAbroad) people to work with.

            // check which scenario are we in:
            var willWontTwoUserCountries = allUsers.GroupBy(user => user.Country).Where(g =>
                g.Count() == 2 && g.Any(user => user.SentAbroad) && g.Any(user => !user.SentAbroad)).ToArray();
            if (willWontTwoUserCountries.Any())
            {
                if (willWontTwoUserCountries.Length > 1)
                {
                    // no problem! cross-match them
                    assignments.AddRange(CrossMatch(willWontTwoUserCountries.SelectMany(g => g)));
                }
                else
                {
                    // problem! we have only one group - we will need to borrow someone from abroad
                    var problem = willWontTwoUserCountries.Single();

                    // do we have a donor?
                    var donor = getDonor(allUsers);
                    if (donor == null)
                    {
                        // sorry! The problematic country folk are going to get removed:
                        foreach (var wontMatch in problem)
                        {
                            abandoned.Add(new Abandoned
                            {
                                SantaUserId = wontMatch.Id,
                                Reason =
                                    "Cannot create at least 3-person ring due to lack of people willing to send abroad"
                            });
                            allUsers.Remove(wontMatch);
                        }
                    }
                    else
                    {
                        // we have a donor!
                        // create assignments - important, do not randomize order or it will not work
                        assignments.AddRange(CreateCircularAssignment(
                            new[]
                            {
                                problem.Single(user => !user.SentAbroad), problem.Single(user => user.SentAbroad), donor
                            }, randomizeOrder: false));

                        // remove users
                        foreach (var user in problem)
                            allUsers.Remove(user);
                        allUsers.Remove(donor);
                    }
                }
            }

            // Ok, we are left with one more edge-case: there might be groups with {1,2} people who WILL send abroad.
            foreach (var oneOrTwoWillCountry in allUsers.GroupBy(user => user.Country)
                .Where(g => g.Count() < 3 && g.All(user => user.SentAbroad)))
            {
                // for each we need to borrow some donors
                var donorsNeeded = 3 - oneOrTwoWillCountry.Count();
                
                // we need a single guy for sure
                var donor = getDonor(allUsers);
                if (donor == null)
                {
                    // sorry! The problematic country folk are going to get removed:
                    foreach (var wontMatch in oneOrTwoWillCountry)
                    {
                        abandoned.Add(new Abandoned
                        {
                            SantaUserId = wontMatch.Id,
                            Reason = "Cannot create at least 3-person ring due to lack of people willing to send abroad"
                        });
                        allUsers.Remove(wontMatch);
                    }
                }
                else
                {
                    if (donorsNeeded == 2)
                    {
                        // and his compatriot
                        var donorCompatriot = allUsers.FirstOrDefault(user => user.Id != donor.Id);
                        if (donorCompatriot == null)
                        {
                            // sorry! The problematic country folk are going to get removed:
                            foreach (var wontMatch in oneOrTwoWillCountry)
                            {
                                abandoned.Add(new Abandoned
                                {
                                    SantaUserId = wontMatch.Id,
                                    Reason = "Cannot create at least 3-person ring due to lack of people willing to send abroad"
                                });
                                allUsers.Remove(wontMatch);
                            }
                        }
                        // all ready, order might be important, so to be safe - use predefined order:
                        assignments.AddRange(CreateCircularAssignment(new[]{ oneOrTwoWillCountry.Single(), donorCompatriot, donor}));

                        // remove all
                        allUsers.Remove(oneOrTwoWillCountry.Single());
                        allUsers.Remove(donorCompatriot);
                        allUsers.Remove(donor);
                    }
                    else
                    {
                        // good to go:
                        assignments.AddRange(CreateCircularAssignment(oneOrTwoWillCountry.Concat(donor.Yield())));
                        foreach (var user in oneOrTwoWillCountry)
                            allUsers.Remove(user);
                        allUsers.Remove(donor);
                    }
                }
            }

            // Let's assert something: every one who is set currently as giver is also a recipient
            var givers = new HashSet<long>(assignments.Select(a => a.GiverId));
            var recipients = new HashSet<long>(assignments.Select(a => a.RecepientId));

            if(!givers.SetEquals(recipients))
                throw new InvalidOperationException("Assertion failed: after intra-country matching not all givers are recipients!");
            
            // let's remove from allUsers people who were matched, and group into country lists
            var usersByCountry = allUsers.Where(user => !givers.Contains(user.Id)).GroupBy(user => user.Country).ToArray();
            
            // assert we are ready to go:
            if (!usersByCountry.All(g=>g.Count() >= 3))
                throw new InvalidOperationException("At this point we should have had only countries with 3+ people in them!");

            foreach(var country in usersByCountry)
                assignments.AddRange(CreateCircularAssignment(country));

            // we should be done - let's do some sanity checks:
            var allUsersById = GetAllUsersWithoutProtectedData().Where(ConfirmedOnly).ToDictionary(user => user.Id, user => user);

            if(allUsersById.Count - abandoned.Count != assignments.Count )
                throw new InvalidOperationException("The assignments number does not equal all users (without the abandoned ones");

            if(!assignments.All(assignment => allUsersById[assignment.GiverId].SentAbroad || allUsersById[assignment.GiverId].Country == allUsersById[assignment.RecepientId].Country))
                throw new InvalidOperationException("Somehow some people are scheduled to send stuff abroad despite selecting that they WILL NOT");

            // nobody can get himself!
            if (assignments.Any(assignment => assignment.GiverId == assignment.RecepientId))
                throw new InvalidOperationException("Somehow somebody got themself!");

            // we can now execute massive amount of inserts ;)
            WithConnection(conn =>
            {
                foreach (var assignment in assignments)
                    conn.Execute(@"INSERT INTO [Assignments] (GiverId, RecepientId) VALUES(@GiverId, @RecepientId)",
                        new { assignment.GiverId, assignment.RecepientId });
            });

            // and save abandoned for future review
            WithConnection(conn =>
            {
                foreach (var abandonedUser in abandoned)
                    conn.Execute(@"INSERT INTO [Abandoned] (SantaUserId, Reason) VALUES(@SantaUserId, @Reason)",
                        new { abandonedUser.SantaUserId, abandonedUser.Reason });
            });

            // verify
            var assignmentsFromDb =
                WithConnection(conn => conn.ExecuteScalar<int>("SELECT COUNT(*) FROM [Assignments]"));

            if (assignmentsFromDb != assignments.Count)
                throw new InvalidOperationException("Inserts were performed, but there is numerical mismatch!");

            return new AssignmentResult
            {
                Abandoned = abandoned,
                Assignments = assignments,
                UserDisplayById = allUsersById
            };

            //// ok, new plan

            //// prepare a list of obtained assignments
            //var assignments = new List<Assignment>(allUsers.Count);

            //// this will be a place for people who sadly cannot be matched
            //var abandoned = new List<Abandoned>();

            //// get linked list (for fast removal) of users, randomize them
            //var targetList = new LinkedList<SantaUser>(allUsers.OrderBy(_ => random.Next()));

            //// and another of people who need matches
            //var gifterList = new LinkedList<SantaUser>(allUsers.OrderBy(_ => random.Next()));

            //// Let's find people in countries where no other users registered
            //var usersAloneInTheirCountries =
            //    allUsers.GroupBy(santaUser => santaUser.Country).Where(g => g.Count() == 1).SelectMany(g => g).ToList();

            //// first check! Will they send abroad? If not, they must be removed! Otherwise we would match someone to them, and yet they couldn't sent gift to anyone!
            //foreach (var loneUser in usersAloneInTheirCountries.ToArray())
            //{
            //    if (!loneUser.SentAbroad)
            //    {
            //        targetList.Remove(loneUser);
            //        gifterList.Remove(loneUser);
            //        usersAloneInTheirCountries.Remove(loneUser);
            //        abandoned.Add(new Abandoned{SantaUserId = loneUser.Id, Reason = "Alone in the country, won't send abroad"});
            //    }
            //}

            //// from now on all should get a match - let's note down numbers:

            //var filteredTargetCount = targetList.Count;
            //var filteredGiverCount = gifterList.Count;

            //// let's match them first so the rest is simpler
            //foreach (var loneUser in usersAloneInTheirCountries)
            //{
            //    // first check! Will they send abroad? If not, they must be removed! Otherwise we would match someone to them, and yet they couldn't sent gift to anyone!


            //    var node = gifterList.First;

            //    //  find not null (duh), will sent abroad, and is not the user we want to get a gift for
            //    while (node != null && !node.Value.SentAbroad && node.Value.Id == loneUser.Id)
            //        node = node.Next;

            //    // do we have aproblem?
            //    if (node == null)
            //    {
            //        // sorry mate, we cannot find you a match. That will happen when there is more lone users than sent abroad users :(
            //        targetList.Remove(loneUser);
            //        gifterList.Remove(loneUser);
            //        // we have removed him from the lottery, let's add him to abandoned, so we know who they were
            //        abandoned.Add(new Abandoned{SantaUserId = loneUser.Id, Reason = "Alone in the country, no one available to send"});
            //        continue;
            //    }

            //    // we found someone! They will no longer be taken into consideration as a giver
            //    gifterList.Remove(node);
            //    // and the target has been matched, he is not getting more!
            //    targetList.Remove(loneUser);

            //    // add assigment for future inserts
            //    assignments.Add(new Assignment{GiverId = node.Value.Id, RecepientId = loneUser.Id});

            //    // ok, the guy got a gift, he is alone in his country, let's find him a match
            //    var loneUserRecipientNode = targetList.First;
            //    while (loneUserRecipientNode != null && loneUserRecipientNode.Value.Id == loneUser.Id)
            //        loneUserRecipientNode = loneUserRecipientNode.Next;

            //    if(loneUserRecipientNode == null)
            //        throw new InvalidOperationException("Something went very wrong - we don't have a target?!");

            //    // we got him a recipient, let's add such assignment
            //    assignments.Add(new Assignment{GiverId = loneUser.Id, RecepientId = loneUserRecipientNode.Value.Id});

            //    // and remove him from givers
            //    gifterList.Remove(loneUser);
            //    // and his target from recipients
            //    targetList.Remove(loneUserRecipientNode);
            //}

            //// by now all people who cannot be matched withing the country have been hopefully dealt with. Let's now do some iterations to match the rest!

            //// asert we have equal number of givers and recipients - if algorithm is correct should not happen
            //if(gifterList.Count != targetList.Count)
            //    throw new InvalidOperationException("Something went very wrong - we don't have same number of givers and targets!");

            //// grouping by country the remaining folk should limit the amount of searching
            //var targetsPerCountry = targetList.GroupBy(target => target.Country)
            //    .ToDictionary(g => g.Key, g => new LinkedList<SantaUser>(g));

            //var giftersPerCountry = gifterList.GroupBy(target => target.Country)
            //    .ToDictionary(g => g.Key, g => g);

            //// start with countries that have most sent-abroads
            //foreach (var country in giftersPerCountry.OrderByDescending(kvp=>kvp.Value.Count(user=>user.SentAbroad)).Select(kvp=>kvp.Key))
            //{
            //    // again, sent-abroads first to solve the issue with single-user countries
            //    foreach (var giver in giftersPerCountry[country].OrderByDescending(user=>user.SentAbroad))
            //    {
            //        LinkedListNode<SantaUser> targetNode;
            //        LinkedList<SantaUser> source;
            //        // so, we can have an anomaly - if there are folk on recipient list that are in a country list all by themselves, we need to take care of them first
            //        if (giver.SentAbroad && targetsPerCountry.Values.Any(list => list.Count == 1))
            //        {
            //            source = targetsPerCountry.Values.First(list => list.Count == 1);
            //            targetNode = source.First;
            //        } else if (!targetsPerCountry[country].Any())
            //        {
            //            // ups, no more people left! let's get from a fullest list... that _might_ work
            //            source = targetsPerCountry.OrderBy(kvp => kvp.Value.Count).First().Value;
            //            targetNode = source.First;
            //        }
            //        else
            //        {
            //            source = targetsPerCountry[country];
            //            // if we are goood, we can do inter-country stuff
            //            targetNode = targetsPerCountry[country].First;

            //            // not null, not self
            //            while (targetNode != null && targetNode.Value.Id == giver.Id)
            //                targetNode = targetNode.Next;
            //        }
            //        // ups?
            //        if (targetNode == null)
            //        {
            //            // shouldn't happen!
            //            throw new InvalidOperationException("We somehow didn't find a target while it was supposed to happen without issue :(");
            //        }

            //        // ok, we have a target for a giver, let's remove target from the list:
            //        source.Remove(targetNode);
            //        // no need to remove gifter, they are iterated!
            //        // create assignment
            //        assignments.Add(new Assignment{GiverId = giver.Id,RecepientId = targetNode.Value.Id});
            //    }
            //}

            //// at this point we should have as many assignments as gifters and targets before our grouping
            //if(assignments.Count != filteredTargetCount || assignments.Count != filteredGiverCount)
            //    throw new InvalidOperationException("Somehow we got less asignments than expected!");

            //// nobody can get himself!
            //if(assignments.Any(assignment=>assignment.GiverId == assignment.RecepientId))
            //    throw new InvalidOperationException("Somehow somebody got themself!");

            //// we can now execute massive amount of inserts ;)
            //WithConnection(conn =>
            //{
            //    foreach (var assignment in assignments)
            //        conn.Execute(@"INSERT INTO [Assignments] (GiverId, RecepientId) VALUES(@GiverId, @RecepientId)",
            //            new {assignment.GiverId, assignment.RecepientId});
            //});

            //// and save abandoned for future review
            //WithConnection(conn =>
            //{
            //    foreach (var abandonedUser in abandoned)
            //        conn.Execute(@"INSERT INTO [Abandoned] (SantaUserId, Reason) VALUES(@SantaUserId, @Reason)",
            //            new { abandonedUser.SantaUserId, abandonedUser.Reason});
            //});

            //// verify
            //var assignmentsFromDb =
            //    WithConnection(conn => conn.ExecuteScalar<int>("SELECT COUNT(*) FROM [Assignments]"));

            //if(assignmentsFromDb != assignments.Count)
            //    throw new InvalidOperationException("Inserts were performed, but there is numerical mismatch!");

            //return new AssignmentResult
            //{
            //    Abandoned = abandoned,
            //    Assignments = assignments,
            //    UserDisplayById = allUsers.ToDictionary(user=>user.Id, user=>user)
            //};
        }

        public long? GetAssignedPartnerIdForUser(long id)
        {
            return WithConnection(
                conn => conn.QuerySingleOrDefault<long?>(@"SELECT RecepientId FROM Assignments WHERE GiverId = @id",
                new {id}));
        }

        public AssignmentResult GetAssignments()
        {
            // get all users
            var allUsers = GetAllUsersWithoutProtectedData();

            // get all assignments
            var allAssignments = WithConnection(conn => conn.Query<Assignment>("SELECT * FROM [Assignments]")).ToArray();

            // get all abandoned
            var allAbandoned = WithConnection(conn => conn.Query<Abandoned>("SELECT * FROM [Abandoned]")).ToArray();

            return new AssignmentResult{Assignments = allAssignments, UserDisplayById = allUsers.ToDictionary(user=>user.Id, user=>user), Abandoned = allAbandoned};
        }

        private T WithConnection<T>(Func<SqlConnection, T> func)
        {
            using (var conn = new SqlConnection(_connectionString))
                return func(conn);
        }

        private void WithConnection(Action<SqlConnection> action)
        {
            using (var conn = new SqlConnection(_connectionString))
                action(conn);
        }
    }
}