namespace PWSH.Kaspa.Base.JSON.Interfaces
{
    public interface IJSONableDisplayable
    {
        /// <summary>
        /// Display as serialized indented JSON.
        /// </summary>
        /// <returns>Indented JSON</returns>
        string ToJSON();
    }
}
