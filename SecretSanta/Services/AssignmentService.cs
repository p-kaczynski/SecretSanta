using System;
using System.Linq;
using SecretSanta.Common.Interface;
using SecretSanta.Common.Result;
using SecretSanta.Domain.Models;

namespace SecretSanta.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAssignmentAlgorithm _algorithm;

        public AssignmentService(IUserRepository userRepository, IAssignmentAlgorithm algorithm)
        {
            _userRepository = userRepository;
            _algorithm = algorithm;
        }

        public AssignmentResult GenerateAssignments()
        {
            // prepare predicate
            bool ConfirmedOnly(SantaUser user) => user.AdminConfirmed && user.EmailConfirmed;

            // get all users
            var allUsers = _userRepository.GetAllUsers().Where(ConfirmedOnly).ToArray();

            if (!allUsers.Any())
                throw new InvalidOperationException("No users have signed up!");

            var result = _algorithm.Assign(allUsers);

            // verify algorithm
            _algorithm.Verify(result); //this will throw for failures

            return result;
        }

        public void SaveAssignments(AssignmentResult result)
            => _userRepository.AssignRecipients(result);

        public bool WasAssigned()
            => _userRepository.WasAssigned();

        public AssignmentResult GetAssignments()
            => _userRepository.GetAssignments();
    }
}