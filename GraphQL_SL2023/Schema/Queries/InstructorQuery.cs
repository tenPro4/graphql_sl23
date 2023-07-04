﻿using GraphQL_SL2023.DTOs;
using GraphQL_SL2023.Models;

namespace GraphQL_SL2023.Schema.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class InstructorQuery
    {
        [UseDbContext(typeof(SchoolDbContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<InstructorType> GetInstructors([ScopedService] SchoolDbContext context)
        {
            return context.Instructors.Select(i => new InstructorType
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Salary = i.Salary,
            });
        }

        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<InstructorType> GetInstructorById(Guid id, [ScopedService] SchoolDbContext context)
        {
            InstructorDTO instructorDTO = await context.Instructors.FindAsync(id);

            if (instructorDTO == null)
            {
                return null;
            }

            return new InstructorType
            {
                Id = instructorDTO.Id,
                FirstName = instructorDTO.FirstName,
                LastName = instructorDTO.LastName,
                Salary = instructorDTO.Salary,
            };
        }
    }
}
