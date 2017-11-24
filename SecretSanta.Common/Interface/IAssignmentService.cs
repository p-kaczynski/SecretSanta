using SecretSanta.Common.Result;

namespace SecretSanta.Common.Interface
{
    public interface IAssignmentService
    {
        AssignmentResult GenerateAssignments();
        void SaveAssignments(AssignmentResult result);
        bool WasAssigned();
        AssignmentResult GetAssignments();
    }
}