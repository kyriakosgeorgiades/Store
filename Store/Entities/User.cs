﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(250)]
    public string UserName { get; set; }

    [Required]
    [MaxLength(250)]
    public string Email { get; set; }

    [Required]
    [MaxLength(600)]
    public string Password { get; set; }
}
