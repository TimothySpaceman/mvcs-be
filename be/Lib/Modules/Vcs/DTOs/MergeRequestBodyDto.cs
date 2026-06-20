using Core.Storage;

namespace Lib.Modules.Vcs.DTOs;

public record MergeRequestBodyDto(
    string Title,
    string TargetRefName,
    string SourceRefName,
    HashId ExpectedTargetHead,
    HashId ExpectedSourceHead
);