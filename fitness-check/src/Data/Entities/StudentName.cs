namespace FitnessCheck.Data.Entities;

/// <summary>
/// Stores the first and last name of a student, keyed by UserId.
/// Populated automatically when a student submits their first attempt.
/// </summary>
public class StudentName
{
    /// <summary>Primary key – the student's identity provider UserId.</summary>
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}
