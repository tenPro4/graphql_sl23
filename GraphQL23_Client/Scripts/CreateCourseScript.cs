using StrawberryShake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL23_Client.Scripts
{
    public class CreateCourseScript
    {
        private readonly IExampleClient _client;

        public CreateCourseScript(IExampleClient client)
        {
            _client = client;
        }

        public async Task Run()
        {
            CourseTypeInput courseInput = new CourseTypeInput()
            {
                Name = "Algebra",
                Subject = Subject.Mathematics,
                InstructorId = Guid.Parse("b0920fe9a1a54468b9faa5fe295dfc2d")
            };

            IOperationResult<ICreateCourseResult> createCourseResult = await _client.CreateCourse.ExecuteAsync(courseInput);

            Console.WriteLine(createCourseResult.Errors);
        }
    }
}
