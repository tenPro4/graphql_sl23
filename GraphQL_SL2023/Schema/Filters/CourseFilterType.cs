using GraphQL_SL2023.Schema.Queries;
using HotChocolate.Data.Filters;

namespace GraphQL_SL2023.Schema.Filters
{
    public class CourseFilterType : FilterInputType<CourseType>
    {
        protected override void Configure(IFilterInputTypeDescriptor<CourseType> descriptor)
        {
            descriptor.Ignore(c => c.Students);

            base.Configure(descriptor);
        }
    }
}
