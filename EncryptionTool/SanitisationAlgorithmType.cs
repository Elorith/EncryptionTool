public enum SanitisationAlgorithmType : int
{
    /// <summary>
    /// Implements the US DoD 5220.22-M (E) data sanitisation algorithm. Overwrites files 3 times. This method offers "high" security but use it only
    /// to erase files that do not contain the very highest level of sensitive information.
    /// </summary>
    DoDFast = 1,
    
    /// <summary>
    /// Implements the US DoD 5220.22-M (ECE) data sanitisation algorithm. Overwrites files 7 times. This method is considered extremely secure and
    /// can be used to erase files that contain the very highest level of sensitive information.
    /// </summary>
    DoDSensitive = 2
}