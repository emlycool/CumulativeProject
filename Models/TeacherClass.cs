using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CumulativeProject.Repositories;

namespace CumulativeProject.Models
{
    public class TeacherClass : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class code is required")]
        [StringLength(50, ErrorMessage = "Class code may not be longer than 50 characters")]
        public string ClassCode { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        public string FormattedStartDate => StartDate.HasValue ? StartDate.Value.ToString("dd/MM/yyyy") : "unknown date";

        [Required(ErrorMessage = "Finish date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FinishDate { get; set; }

        public string FormattedFinishDate => FinishDate.HasValue ? FinishDate.Value.ToString("dd/MM/yyyy") : "unknown date";

        [Required(ErrorMessage = "Class name is required")]
        [StringLength(50, ErrorMessage = "Class name may not be longer than 50 characters")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Teacher ID is required")]
        public int? TeacherId { get; set; }

        public Teacher Teacher { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && FinishDate.HasValue && StartDate > FinishDate)
            {
                yield return new ValidationResult("Start date cannot be after finish date", new[] { nameof(StartDate), nameof(FinishDate) });
            }


            if (TeacherId != null)
            {
                Teacher teacher = (new TeacherRepository()).Find((int)TeacherId);
                if (teacher == null)
                {
                    yield return new ValidationResult("Teacher with the specified ID does not exist", new[] { nameof(TeacherId) });
                }
            }

        }
    }
}
