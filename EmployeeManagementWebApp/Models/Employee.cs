﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementWebApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Display(Name = "Office Email")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required]
        public Dept? Department { get; set; }
        public string PhotoPath { get; set; }
    }
}
