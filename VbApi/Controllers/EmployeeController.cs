using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace VbApi.Controllers
{

    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(250)
                .WithMessage("Invalid Name");

            RuleFor(x => x.DateOfBirth)
                .Must(BeValidBirthDate)
                .WithMessage("Birthdate is not valid.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Email address is not valid.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .Must(BeValidPhoneNumber)
                .WithMessage("Phone is not valid.");

            RuleFor(x => x.HourlySalary)
                .Must(BeValidHourlySalary)
                .WithMessage("Minimum hourly salary is not valid.");
        }

        private bool BeValidBirthDate(DateTime dateOfBirth)
        {
            var minAllowedBirthDate = DateTime.Today.AddYears(-65);
            return minAllowedBirthDate <= dateOfBirth;
        }

        private bool BeValidPhoneNumber(string phone)
        {
            
            return !string.IsNullOrWhiteSpace(phone) && phone.Length >= 10;
        }

        private bool BeValidHourlySalary(Employee employee, double hourlySalary)
        {
            var dateBeforeThirtyYears = DateTime.Today.AddYears(-30);
            var isOlderThanThirtyYears = employee.DateOfBirth <= dateBeforeThirtyYears;

            return isOlderThanThirtyYears ? hourlySalary >= employee.MinSeniorSalary : hourlySalary >= employee.MinJuniorSalary;
        }
    }

    public class Employee : IValidatableObject
    {
        public string name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public double HourlySalary { get; set; }
        public double MinJuniorSalary { get; set; }
        public double MinSeniorSalary { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            var minAllowedBirthDate = DateTime.Today.AddYears(-65);
            if (minAllowedBirthDate > DateOfBirth)
            {
                yield return new ValidationResult("Birthdate is not valid.");
            }
        }
        }

        [Route("api/[controller]")]
        [ApiController]
        public class EmployeeController : ControllerBase
        {
            private readonly EmployeeValidator _validator;

            public EmployeeController(EmployeeValidator validator)
            {
                _validator = validator;
            }

            [HttpPost]
            public IActionResult Post([FromBody] Employee value)
            {
                var validationResult = _validator.Validate(value);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
                
                if (value.DateOfBirth > DateTime.Now.AddYears(-30) && value.HourlySalary < 200)
                {
                }

                return Ok(value);
            }
        }







    }
    