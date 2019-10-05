using System;
using System.Linq;
using System.Reflection;
using SecretSanta.Common.Helpers;
using SecretSanta.Domain.Attributes;
using SecretSanta.Domain.Enums;
using SecretSanta.Domain.Models;
using FluentAssertions;
using Xunit;

namespace SecretSanta.Common.Tests
{
    public class DataProtectionTests
    {
        [Fact]
        public void ClearDataProtected_ClearsAllFields()
        {
            var user = new SantaUser
            {
                Id = 1,
                Country = "abc",
                SendAbroad = SendAbroadOption.Want,
                AddressLine1 = "a1",
                AddressLine2 = "a2",
                AdminConfirmed = false,
                City = "c",
                CreateDate = DateTime.Now,
                DisplayName = "d",
                Email = "e",
                EmailConfirmed = false,
                FacebookProfileUrl = "f",
                FullName = "fn",
                Note = "n",
                PasswordHash = new byte[10],
                PostalCode = "pc"
            };

            user.ClearDataProtected();
            foreach (var dataProtectedProperty in typeof(SantaUser)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute<DataProtectionAttribute>() != null))
            {
                dataProtectedProperty.GetValue(user).Should().BeNull();
            }
        }
    }
}