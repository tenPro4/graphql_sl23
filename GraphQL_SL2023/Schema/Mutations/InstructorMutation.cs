using GraphQL_SL2023.DTOs;
using GraphQL_SL2023.Models;
using GraphQL_SL2023.Schema.Subscriptions;
using HotChocolate.Subscriptions;

namespace GraphQL_SL2023.Schema.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class InstructorMutation
    {
        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<InstructorResult> CreateInstructor(
          InstructorTypeInput instructorInput,
          [ScopedService] SchoolDbContext context,
          [Service] ITopicEventSender topicEventSender)
        {
            InstructorDTO instructorDTO = new InstructorDTO()
            {
                FirstName = instructorInput.FirstName,
                LastName = instructorInput.LastName,
                Salary = instructorInput.Salary,
            };

            context.Add(instructorDTO);
            await context.SaveChangesAsync();

            InstructorResult instructorResult = new InstructorResult()
            {
                Id = instructorDTO.Id,
                FirstName = instructorDTO.FirstName,
                LastName = instructorDTO.LastName,
                Salary = instructorDTO.Salary,
            };

            await topicEventSender.SendAsync(nameof(Subscription.InstructorCreated), instructorResult);

            return instructorResult;
        }
    }
}
