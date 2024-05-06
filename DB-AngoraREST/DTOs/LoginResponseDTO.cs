namespace DB_AngoraREST.DTOs
{
    public record LoginResponseDTO
    {
        public bool Success { get; init; }
        public string Token { get; init; }
        public string ErrorMessage { get; init; }
    }
}
