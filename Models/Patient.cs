using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class Patient
{
    public string Ssn { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    
    [NotMapped]
    public Measurement[] Measurements { get; set; }
}