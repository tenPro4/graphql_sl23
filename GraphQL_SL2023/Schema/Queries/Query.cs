using Bogus;
using GraphQL_SL2023.Models;
using GraphQL_SL2023.Schema.Filters;
using GraphQL_SL2023.Schema.Sorters;
using GraphQL_SL2023.Services.Courses;
using Microsoft.EntityFrameworkCore;

namespace GraphQL_SL2023.Schema.Queries
{
    public class Query
    {
        private readonly Faker<InstructorType> _instructorFaker;
        private readonly Faker<StudentType> _studentFaker;
        private readonly Faker<CourseType> _courseFaker;

        private readonly CoursesRepository _coursesRepository;

        public Query(CoursesRepository coursesRepository)
        {
            _instructorFaker = new Faker<InstructorType>()
               .RuleFor(x => x.Id, f => Guid.NewGuid())
               .RuleFor(x => x.FirstName, f => f.Name.FirstName())
               .RuleFor(x => x.LastName, f => f.Name.LastName())
               .RuleFor(x => x.Salary, f => f.Random.Double(0, 10000));

            _studentFaker = new Faker<StudentType>()
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.GPA, f => f.Random.Double(1, 4));

            _courseFaker = new Faker<CourseType>()
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.Name, f => f.Name.JobTitle())
                .RuleFor(x => x.Subject, f => f.PickRandom<Subject>())
                .RuleFor(x => x.Students, f => _studentFaker.Generate(3));

            _coursesRepository = coursesRepository;
        }

        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<IEnumerable<ISearchResultType>> Search(string term, [ScopedService] SchoolDbContext context)
        {
            IEnumerable<CourseType> courses = await context.Courses
                .Where(c => c.Name.Contains(term))
                .Select(c => new CourseType()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Subject = c.Subject,
                    InstructorId = c.InstructorId,
                    CreatorId = c.CreatorId
                })
                .ToListAsync();

            IEnumerable<InstructorType> instructors = await context.Instructors
                .Where(i => i.FirstName.Contains(term) || i.LastName.Contains(term))
                .Select(i => new InstructorType()
                {
                    Id = i.Id,
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    Salary = i.Salary,
                })
                .ToListAsync();

            return new List<ISearchResultType>()
                .Concat(courses)
                .Concat(instructors);
        }

        [UsePaging(IncludeTotalCount =true,DefaultPageSize =10)]
        public async Task<IEnumerable<CourseType>> GetCoursesRep()
        {
            var courses = await _coursesRepository.GetAll();

            return courses.Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId= c.InstructorId,
            });
        }

        [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 10)]
        public async Task<IEnumerable<CourseType>> GetOffsetCoursesRep()
        {
            var courses = await _coursesRepository.GetAll();

            return courses.Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId,
            });
        }

        [UseDbContext(typeof(SchoolDbContext))]
        [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10)]
        //[UseFiltering]
        [UseProjection]
        [UseFiltering(typeof(CourseFilterType))]
        //[UseSorting]
        [UseSorting(typeof(CourseSortType))]
        public async Task<IQueryable<CourseType>> GetCoursesPagination([ScopedService]SchoolDbContext context)
        {
            var courses = await _coursesRepository.GetAll();

            return context.Courses.Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId,
            });
        }

        public IEnumerable<CourseType> GetCourses()
        {
            return _courseFaker.Generate(3);
        }

        //public async Task<CourseType> GetCourseByIdAsync(Guid id)
        //{
        //    await Task.Delay(1000);
        //    var course = _courseFaker.Generate();

        //    course.Id = id;

        //    return course;
        //}


        [GraphQLDeprecated("This query is deprecated.")]
        public string TestInstructions => "Smash that like button and subscribe to SingletonSean";
    }
}
