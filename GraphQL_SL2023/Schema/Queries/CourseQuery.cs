using GraphQL_SL2023.DTOs;
using GraphQL_SL2023.Models;
using GraphQL_SL2023.Schema.Filters;
using GraphQL_SL2023.Schema.Sorters;
using Microsoft.EntityFrameworkCore;

namespace GraphQL_SL2023.Schema.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class CourseQuery
    {
        [UseDbContext(typeof(SchoolDbContext))]
        [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10)]
        [UseProjection]
        [UseFiltering(typeof(CourseFilterType))]
        [UseSorting(typeof(CourseSortType))]
        public IQueryable<CourseType> GetCourses([ScopedService] SchoolDbContext context)
        {
            return context.Courses.Include(x => x.Students).Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId,
                CreatorId = c.CreatorId
            });
        }

        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<CourseType> GetCourseByIdAsync(Guid id, [ScopedService] SchoolDbContext context)
        {
            CourseDTO courseDTO = await context.Courses.FindAsync(id);

            if (courseDTO == null)
            {
                return null;
            }

            return new CourseType()
            {
                Id = courseDTO.Id,
                Name = courseDTO.Name,
                Subject = courseDTO.Subject,
                InstructorId = courseDTO.InstructorId,
                CreatorId = courseDTO.CreatorId
            };
        }
    }
}
