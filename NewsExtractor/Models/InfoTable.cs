﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NewsExtractor.Models;

[Table("InfoTable")]
public partial class InfoTable
{
    [Key]
    public int Id { get; set; }

    public bool? Status { get; set; }

    public string SectionName { get; set; }

    public string Date { get; set; }

    public string Hours { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string AllInfo { get; set; }

    public string ImageBytes { get; set; }
}