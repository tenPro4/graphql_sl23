using FluentValidation;
using GraphQL_SL2023.Schema.Mutations;

namespace GraphQL_SL2023.Validators
{
    public class InstructorTypeInputValidator : AbstractValidator<InstructorTypeInput>
    {
        public InstructorTypeInputValidator()
        {
            RuleFor(i => i.FirstName).NotEmpty();
            RuleFor(i => i.LastName).NotEmpty();
        }
    }
}
