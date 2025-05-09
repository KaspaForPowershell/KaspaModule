﻿namespace PWSH.Kaspa.Verbs;

public sealed partial class GetUTXOsForAddresses
{
    [ValidateKaspaAddress]
    [Parameter(Mandatory = true, HelpMessage = "Specify addresses.")]
    public List<string>? Addresses { get; set; }

    [Parameter(Mandatory = false, HelpMessage = "Http client timeout.")]
    public ulong TimeoutSeconds { get; set; } = Globals.DEFAULT_TIMEOUT_SECONDS;

    [Parameter(Mandatory = false)]
    public SwitchParameter AsJob { get; set; }
}
