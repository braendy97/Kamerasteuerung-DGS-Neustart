using Kamerasteuerung.DGS.Core.Models;

namespace Kamerasteuerung.DGS.Core.Services;

public static class ParticipantListService
{
    public static IReadOnlyList<ParticipantEntry> SortAlphabetically(IEnumerable<ParticipantEntry> participants) =>
        participants
            .OrderBy(p => p.DisplayName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
}
