using GraphQL_SL2023.Models;
using GraphQL_SL2023.Schema.Queries;
using GraphQL_SL2023.Schema.Subscriptions;
using HotChocolate.Subscriptions;

namespace GraphQL_SL2023.Schema.Mutations
{
    public class Mutation
    {
        private readonly List<CourseResult> _courses;

        public Mutation()
        {
            _courses = new List<CourseResult>();
        }

        //public async Task<CourseResult> CreateCourse(CourseResult input, [Service] ITopicEventSender topicEventSender)
        //{
        //    var c = new CourseResult
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = input.Name,
        //        Subject = input.Subject,
        //        InstructorId = input.InstructorId
        //    };
        //    _courses.Add(c);

        //    await topicEventSender.SendAsync(nameof(Subscription.CourseCreated), c);

        //    return c;
        //}

        public async Task<CourseResult> UpdateCourse(CourseResult input, [Service] ITopicEventSender topicEventSender)
        {
            var course = _courses.FirstOrDefault(c => c.Id == input.Id);

            if(course == null)
            {
                //throw new Exception("Course not found");
                throw new GraphQLException(new Error("Course not found", "COURSE_NOT_FOUND"));
            }

            course.Name = input.Name;
            string topic = $"{course.Id}_{nameof(Subscription.CourseUpdated)}";
            await topicEventSender.SendAsync(topic, course);

            return course;
        }
    }
}
