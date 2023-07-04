using AppAny.HotChocolate.FluentValidation;
using GraphQL_SL2023.DTOs;
using GraphQL_SL2023.Middlewares.UseUser;
using GraphQL_SL2023.Models;
using GraphQL_SL2023.Schema.Filters;
using GraphQL_SL2023.Schema.Subscriptions;
using GraphQL_SL2023.Services.Courses;
using GraphQL_SL2023.Validators;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;
using System.ComponentModel.DataAnnotations;

namespace GraphQL_SL2023.Schema.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class CourseMutation
    {
        private readonly CoursesRepository _coursesRepository;
        private readonly CourseTypeInputValidator _typeInputValidator;

        public CourseMutation(CoursesRepository coursesRepository, CourseTypeInputValidator typeInputValidator)
        {
            _coursesRepository = coursesRepository;
            _typeInputValidator = typeInputValidator;
        }

        [Authorize(Policy = "IsAdmin")]
        [UseUser]
        public async Task<CourseResult> CreateCourse(
            [UseFluentValidation, UseValidator<CourseTypeInputValidator>] CourseTypeInput courseInput,
            [Service] ITopicEventSender topicEventSender,
            [User] User user)
        {
            //Validate(courseInput);

            CourseDTO courseDTO = new CourseDTO()
            {
                Name = courseInput.Name,
                Subject = courseInput.Subject,
                InstructorId = courseInput.InstructorId,
                CreatorId = user.Id
            };

            courseDTO = await _coursesRepository.Create(courseDTO);

            CourseResult course = new CourseResult()
            {
                Id = courseDTO.Id,
                Name = courseDTO.Name,
                Subject = courseDTO.Subject,
                InstructorId = courseDTO.InstructorId
            };

            await topicEventSender.SendAsync(nameof(Subscription.CourseCreated), course);

            return course;
        }

        private void Validate(CourseTypeInput courseInput)
        {
            var validationResult = _typeInputValidator.Validate(courseInput);

            if(!validationResult.IsValid)
            {
                throw new GraphQLException("Invalid input");
            }
        }

        public async Task<CourseResult> UpdateCourse(Guid id,
           CourseTypeInput courseInput,
            [Service] ITopicEventSender topicEventSender)
        {
            string userId = "1";

            CourseDTO courseDTO = await _coursesRepository.GetById(id);

            if (courseDTO == null)
            {
                throw new GraphQLException(new Error("Course not found.", "COURSE_NOT_FOUND"));
            }

            if (courseDTO.CreatorId != userId)
            {
                throw new GraphQLException(new Error("You do not have permission to update this course.", "INVALID_PERMISSION"));
            }

            courseDTO.Name = courseInput.Name;
            courseDTO.Subject = courseInput.Subject;
            courseDTO.InstructorId = courseInput.InstructorId;

            courseDTO = await _coursesRepository.Update(courseDTO);

            CourseResult course = new CourseResult()
            {
                Id = courseDTO.Id,
                Name = courseDTO.Name,
                Subject = courseDTO.Subject,
                InstructorId = courseDTO.InstructorId
            };

            string updateCourseTopic = $"{course.Id}_{nameof(Subscription.CourseUpdated)}";
            await topicEventSender.SendAsync(updateCourseTopic, course);

            return course;
        }
        
        public async Task<bool> DeleteCourse(Guid id)
        {
            try
            {
                return await _coursesRepository.Delete(id);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
