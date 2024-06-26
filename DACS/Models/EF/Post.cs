﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DACS.Models.EF
{
    public class Post : CommonAbstract
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(150, ErrorMessage = "Không được vượt quá 150 ký tự")]
        public string Title { get; set; }
        public string? Alias { get; set; }
        public string Detail { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }


        public bool IsActive { get; set; }
        public virtual Category? Category { get; set; }

    }
    
}
