using System.Management.Automation;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    /// <summary>
    /// The smallest unit of Kaspa.
    /// </summary>
    [Cmdlet(KaspaVerbNames.Get, "SompiPerKaspa")]
    [OutputType(typeof(uint))]
    public sealed class GetSompiPerKaspa : KaspaPSCmdlet
    {
        private const uint VALUE = 100000000;

        protected override void EndProcessing()
            => WriteObject(VALUE);
    }
}
