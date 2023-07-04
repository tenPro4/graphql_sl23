﻿using GraphQL_SL2023.DataLoaders;
using GraphQL_SL2023.DTOs;
using GraphQL_SL2023.Models;

namespace GraphQL_SL2023.Schema.Queries
{
    public class CourseType : ISearchResultType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Subject Subject { get; set; }
        [IsProjected(true)]
        public Guid InstructorId { get; set; }

        [GraphQLNonNullType]
        public async Task<InstructorType> Instructor([Service] InstructorDataLoader instructorDataLoader)
        {
            InstructorDTO instructorDTO = await instructorDataLoader.LoadAsync(InstructorId, CancellationToken.None);

            return new InstructorType()
            {
                Id = instructorDTO.Id,
                FirstName = instructorDTO.FirstName,
                LastName = instructorDTO.LastName,
                Salary = instructorDTO.Salary,
            };
        }
        public IEnumerable<StudentType> Students { get; set; }

        [IsProjected(true)]
        public string CreatorId { get; set; }

        public async Task<UserType> Creator([Service] UserDataLoader userDataLoader)
        {
            if (CreatorId == null)
            {
                return null;
            }

            return await userDataLoader.LoadAsync(CreatorId, CancellationToken.None);
        }
    }
}
