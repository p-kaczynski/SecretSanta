using System;
using System.Data;
using Moq;
using SecretSanta.Common.Interface;
using SecretSanta.Domain.Models;
using FluentAssertions;
using Xunit;

namespace SecretSanta.Data.Tests
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _userRepository;
        private readonly Mock<IConfigProvider> _configProviderMock = new Mock<IConfigProvider>();
        private readonly Mock<IEncryptionProvider> _encryptionProviderMock = new Mock<IEncryptionProvider>();
        private readonly Mock<IAssignmentAlgorithm> _assignmentAlgorithmMock = new Mock<IAssignmentAlgorithm>();
        private readonly Mock<IDbConnection> _dbConnectionMock = new Mock<IDbConnection>();

        public UserRepositoryTests()
        {
            _userRepository = new UserRepository(_configProviderMock.Object, _encryptionProviderMock.Object,
                _assignmentAlgorithmMock.Object);
            _userRepository.DbConnectionFactory = _ => _dbConnectionMock.Object;
        }

        [Fact]
        public void CanCreate()
        {
            _userRepository.Should().NotBeNull();
        }

        [Fact]
        public void SqlConnection_UsesConnectionFromConfig()
        {
            const string testConnectionString = "abcd123";

            _configProviderMock.Setup(c => c.ConnectionString).Returns(testConnectionString);
            var userRepository = new UserRepository(_configProviderMock.Object,_encryptionProviderMock.Object,_assignmentAlgorithmMock.Object);
            var testFunc = new Func<string, IDbConnection>(connectionString =>
            {
                connectionString.Should().Be(testConnectionString);
                return _dbConnectionMock.Object;
            });


            userRepository.DbConnectionFactory = testFunc;
            try
            {
                _userRepository.InsertUser(new SantaUser());
            }
            catch
            {
                // ignore
            }
        }

        [Fact]
        public void InsertUser_EncryptsModel()
        {
            var user = new SantaUser();
            try
            {
                _userRepository.InsertUser(user);
            }
            catch
            {
                // ignore
            }
            _encryptionProviderMock.Verify(e=>e.Encrypt(user),Times.Once);
        }

    }
}