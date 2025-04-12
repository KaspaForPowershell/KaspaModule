namespace PWSH.Kaspa.Base
{
    public enum KaspaResolvePreviousOutpointsOption
    {
        No,
        /// <summary>
        /// Fetches only the address and amount.
        /// </summary>
        Light,
        /// <summary>
        /// Fetches the whole TransactionOutput and adds it into each TxInput.
        /// </summary>
        Full
    }
}
