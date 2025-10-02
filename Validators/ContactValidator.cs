using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using FluentValidation;

namespace be_devextreme_starter.Validators
{
    public class ContactValidator: AbstractValidator<ContactDTO>
    {
        private readonly DataEntities _db;
        public ContactValidator(DataEntities db)
        {
            _db = db;

            RuleFor(c => c.full_name)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters.");

            RuleFor(c => c.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(c => c.phone_number)
                .MaximumLength(25).WithMessage("Phone number cannot exceed 25 characters.");

            RuleFor(c => c.company)
                .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters.");

            RuleFor(c => c.job_title)
                .MaximumLength(100).WithMessage("Job title cannot exceed 100 characters.");
            
            RuleFor(c => c.address)
                .MaximumLength(255).WithMessage("Address cannot exceed 255 characters.");

            RuleFor(c => c.city_id)
                .GreaterThan(0).When(c => c.city_id.HasValue)
                .WithMessage("City ID must be a positive integer.")
                .Must(x => _db.City_Masters.Any(c => c.city_id == x))
                .WithMessage("City ID does not exist.");

            RuleFor(c => c.postal_code)
                .InclusiveBetween(10000, 99999).When(c => c.postal_code.HasValue)
                .WithMessage("Postal code must be between 10000 and 99999.");

            RuleFor(c => c.date_added)
                .NotEmpty().WithMessage("Date added is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Date added cannot be in the future.");

            RuleFor(c => c.last_contacted_date)
                .LessThanOrEqualTo(DateTime.Now).When(c => c.last_contacted_date.HasValue)
                .WithMessage("Last contacted date cannot be in the future.");

            RuleFor(c => c.lead_source_id)
                .GreaterThan(0).When(c => c.lead_source_id.HasValue)
                .WithMessage("Lead source ID must be a positive integer.")
                .Must(x => _db.Lead_Source_Masters.Any(l => l.lead_source_id == x))
                .WithMessage("Lead source ID does not exist.");

            RuleFor(c => c.contact_status_id)
                .GreaterThan(0).When(c => c.contact_status_id.HasValue)
                .WithMessage("Contact status ID must be a positive integer.")
                .Must(x => _db.Contact_Status_Masters.Any(cs => cs.contact_status_id == x))
                .WithMessage("Contact status ID does not exist.");

            RuleFor(c => c.estimated_value)
                .GreaterThanOrEqualTo(0).When(c => c.estimated_value.HasValue)
                .WithMessage("Estimated value must be non-negative.");

            RuleFor(c => c.is_subscribed)
                .NotNull().WithMessage("Is subscribed is required.");

            RuleFor(c => c.notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
