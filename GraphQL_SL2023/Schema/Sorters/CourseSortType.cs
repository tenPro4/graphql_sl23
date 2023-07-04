using GraphQL_SL2023.Schema.Queries;
using HotChocolate.Data.Sorting;

namespace GraphQL_SL2023.Schema.Sorters
{
    public class CourseSortType : SortInputType<CourseType>
    {
        protected override void Configure(ISortInputTypeDescriptor<CourseType> descriptor)
        {
            descriptor.Ignore(c => c.Id);
            descriptor.Ignore(c => c.InstructorId);

            base.Configure(descriptor);
        }
    }
}
