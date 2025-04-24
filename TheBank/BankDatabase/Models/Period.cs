namespace BankDatabase.Models;

class Period
{
    public required string Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public required string StorekeeperId { get; set; }
}
