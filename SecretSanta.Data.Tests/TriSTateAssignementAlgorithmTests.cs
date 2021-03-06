﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace SecretSanta.Data.Tests
{
    public class TriSTateAssignementAlgorithmTests
    {
        private static readonly Random Random = new Random();
        private static long _idSource = 0;

        private readonly ITestOutputHelper output;

        // demographic settings - chance 1 in ... to
        private const int IsAbroad = 13;
        private const int InPolandCanSendAbroad = 5;
        private const int InPolandCanAndWantSendAbroad = 5;
        private const int IsAbroadWantSendAbroad = 2;

        private static readonly Dictionary<string, int> Countries = new Dictionary<string, int>
        {
            {"gbr", 29 },
            {"deu", 9 },
            {"usa", 5 },
            {"dnk",3 },
            {"bel", 2 },
            {"nld", 2 },
            {"ita", 1 },
            {"fra", 1 },
            {"geo", 1},
            {"isl", 1 },
            {"svk", 1 }
        };

        private static readonly int TotalCountryWeight = Countries.Values.Sum();

        private readonly TriStateAssignmentAlgorithm _algorithm = new TriStateAssignmentAlgorithm();


        public TriSTateAssignementAlgorithmTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        // === TESTS ===

        [Theory]
        [MemberData(nameof(SantaUserProvider),20)]
        public void CreateAssignment(ICollection<SantaUser> users)
        {
            var result = _algorithm.Assign(users);
            output.WriteLine($"Success={result.Success}, user count={users.Count}, assignments={result.Assignments.Count}, abandoned={result.Abandoned.Count} (Loner:{result.Abandoned.Count(a=>a.Reason == AbandonmentReason.LoneWontSend)}, Algorithm: {result.Abandoned.Count(a=>a.Reason == AbandonmentReason.ComputerSaysNo)})");

            // 1. Numbers should match
            (result.Assignments.Count + result.Abandoned.Count).Should().Be(users.Count);
            output.WriteLine("Missing people: Not detected");
            
            // 2. Nobody should be sending to themselves
            result.Assignments.All(a=>a.RecepientId != a.GiverId).Should().BeTrue();
            output.WriteLine("Sending to yourself: Not detected");

            // 3. Nobody should be sending abroad if they did not wish to
            var lookup = users.ToDictionary(u => u.Id, u => u);
            foreach(var a in result.Assignments)
            {
                var giver = lookup[a.GiverId];
                var recipient = lookup[a.RecepientId];
                if(giver.SendAbroad == SendAbroadOption.WillNot)
                    giver.Country.Should().Be(recipient.Country);
            }
            output.WriteLine("Sending abroad against preferences: Not detected");

            // 4. Everyone who sends, gets
            var givers = new HashSet<long>(result.Assignments.Select(a => a.GiverId));
            var recipients  = new HashSet<long>(result.Assignments.Select(a => a.RecepientId));
            givers.SetEquals(recipients).Should().BeTrue();
            output.WriteLine("Unequal exchange: Not detected");

            // 5. Duplicates?
            result.Assignments.Select(a=>a.RecepientId).Distinct().Count().Should().Be(result.Assignments.Select(a=>a.RecepientId).Count());
            output.WriteLine("Duplicates: Not detected");

            var canSendAbroad = result.Assignments
                .Select(a => new
                {
                    Preference = lookup[a.GiverId].SendAbroad,
                    SendsAbroad = lookup[a.GiverId].Country != lookup[a.RecepientId].Country
                }).Where(o => o.Preference == SendAbroadOption.Can).ToArray();
            var willSendAbroad = (double)canSendAbroad.Count(o => o.SendsAbroad);
            output.WriteLine($"Percentage of people who will send abroad though they rather wouldn't: {willSendAbroad/canSendAbroad.Length:P} ({willSendAbroad}/{canSendAbroad.Length})");

        }

        [Theory]
        [MemberData(nameof(SantaUserProvider), 20)]
        public void Verify(ICollection<SantaUser> users)
        {
            var result = _algorithm.Assign(users);
            result.UserDisplayById = users.ToDictionary(u => u.Id, u => u);
            output.WriteLine($"Success={result.Success}, user count={users.Count}, assignments={result.Assignments.Count}, abandoned={result.Abandoned.Count} (Loner:{result.Abandoned.Count(a => a.Reason == AbandonmentReason.LoneWontSend)}, Algorithm: {result.Abandoned.Count(a => a.Reason == AbandonmentReason.ComputerSaysNo)})");
            
            // this throws on errors.
            _algorithm.Verify(result);

        }

        #region SupportingCode

        public static IEnumerable<object[]> SantaUserProvider(int setCount)
        {
            for (var i = 0; i < setCount; ++i)
                yield return new object[] {GenerateRandomUsers(Random.Next(20,2000)).ToArray()};
        }

        private static IEnumerable<SantaUser> GenerateRandomUsers(int howMany)
        {
            for (var i = 0; i < howMany; ++i)
                yield return GetRandomUser();
        }

        private static SantaUser GetRandomUser()
        {
            var isAbroad = Random.Next(0, IsAbroad) == 0;
            SendAbroadOption sendOption = SendAbroadOption.WillNot;
            if (isAbroad)
            {
                if (Random.Next(0, IsAbroadWantSendAbroad) == 0)
                sendOption = SendAbroadOption.Want;
            }
            else
            {
                if (Random.Next(0, InPolandCanSendAbroad) == 0)
                {
                    if (Random.Next(0, InPolandCanAndWantSendAbroad) == 0)
                        sendOption = SendAbroadOption.Want;
                    else
                        sendOption = SendAbroadOption.Can;
                }
                else sendOption = SendAbroadOption.WillNot;
            }
            return new SantaUser
            {
                // for assignment we care only about id, send option and country
                Id = Interlocked.Increment(ref _idSource),
                Country = isAbroad ? GetCountry() : "pol",
                SendAbroad = sendOption
            };
        }

        private static string GetCountry()
        {
            var rand = Random.Next(0, TotalCountryWeight);
            foreach (var kvp in Countries)
            {
                if (rand < kvp.Value)
                    return kvp.Key;
                rand = rand - kvp.Value;
            }
            return "pry"; // shouldn't happen me thinks.
        }
        #endregion
    }
}