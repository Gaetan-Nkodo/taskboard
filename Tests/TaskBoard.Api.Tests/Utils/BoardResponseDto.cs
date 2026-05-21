public sealed record BoardResponseDto(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<ColumnResponseDto> Columns
);

public sealed record ColumnResponseDto(
    Guid Id,
    string Name,
    int Order,
    IReadOnlyList<TaskResponseDto> Tasks
);

public sealed record TaskResponseDto(
    Guid Id,
    string Name,
    string? Description,
    string? Icon,
    int Order
);
