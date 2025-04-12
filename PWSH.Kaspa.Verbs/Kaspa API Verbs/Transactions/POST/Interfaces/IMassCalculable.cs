namespace PWSH.Kaspa.Verbs.Interfaces
{
    public interface IMassCalculable
    {
        /// <summary>
        /// Helper method to convert any block transaction schema to the one that can be used for mass calculation.
        /// For now all reponse schemas are per command, to avoid API instability. 
        /// So this is a small workaround, to ease the pain of passing them to mass calculation command.
        /// </summary>
        CalculateTransactionMass.RequestSchema ToMassRequestSchema(); // TODO: Remove it when the API will stabilise.
    }
}
