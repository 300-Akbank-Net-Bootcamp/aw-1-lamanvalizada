using FluentValidation;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;

namespace VbApi.Controllers
{
    public class StaffValidator : AbstractValidator<Staff>
    {
        public StaffValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(250);
            
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^\d{10}$");

            RuleFor(x => x.HourlySalary)
                .InclusiveBetween(30, 400);
        }
    }

    public class Staff
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal? HourlySalary { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IValidator<Staff> _validator;

        public StaffController(IValidator<Staff> validator)
        {
            _validator = validator;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Staff value)
        {
            var validationResult = _validator.Validate(value);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            return Ok(value);
        }
    }
}