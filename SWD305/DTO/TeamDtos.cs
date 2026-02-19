namespace SWD305.DTO
{
    public class CreateTeamDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class JoinTeamDto
    {
        public string InviteCode { get; set; } = null!;
    }
}

