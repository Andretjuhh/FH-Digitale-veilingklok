namespace Application.DTOs.Input;

public class DeleteAccountRequestDTO
{
    public Guid AccountId { get; set; }
    public bool HardDelete { get; set; } // true = hard delete, false = soft delete
}
