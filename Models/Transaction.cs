namespace Models;

public record Transaction(Guid Id,string Type, decimal Amount, DateTime Date);
