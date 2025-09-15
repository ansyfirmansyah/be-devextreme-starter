using System;
using System.Collections.Generic;

namespace be_devextreme_starter.Data.Models;

public partial class Time
{
    public DateTime PK_Date { get; set; }

    public string? Date_Name { get; set; }

    public DateTime? Year { get; set; }

    public string? Year_Name { get; set; }

    public DateTime? Quarter { get; set; }

    public string? Quarter_Name { get; set; }

    public DateTime? Month { get; set; }

    public string? Month_Name { get; set; }

    public DateTime? Week { get; set; }

    public string? Week_Name { get; set; }

    public int? Day_Of_Year { get; set; }

    public string? Day_Of_Year_Name { get; set; }

    public int? Day_Of_Quarter { get; set; }

    public string? Day_Of_Quarter_Name { get; set; }

    public int? Day_Of_Month { get; set; }

    public string? Day_Of_Month_Name { get; set; }

    public int? Day_Of_Week { get; set; }

    public string? Day_Of_Week_Name { get; set; }

    public int? Week_Of_Year { get; set; }

    public string? Week_Of_Year_Name { get; set; }

    public int? Month_Of_Year { get; set; }

    public string? Month_Of_Year_Name { get; set; }

    public int? Month_Of_Quarter { get; set; }

    public string? Month_Of_Quarter_Name { get; set; }

    public int? Quarter_Of_Year { get; set; }

    public string? Quarter_Of_Year_Name { get; set; }
}
