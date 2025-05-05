namespace PWSH.Kaspa.Verbs;

/// <summary>
/// Convert timespan to date.
/// </summary>
[Cmdlet(KaspaVerbNames.ConvertFrom, "Timestamp")]
[OutputType(typeof(DateTimeOffset))]
public sealed partial class ConvertFromTimestamp : KaspaPSCmdlet
{
    protected override void EndProcessing()
        => WriteObject(DateTimeOffset.FromUnixTimeMilliseconds(Timestamp));
}
