public enum SanitisationAlgorithmType : int
{
    /// <summary>
    /// Implements the US DoD 5220.22-M (E) data sanitisation algorithm. Overwrites files 3 times. This method offers "medium" security. Use it only
    /// to erase files that do not contain highly sensitive information.
    /// </summary>
    DoDFast = 1,
    
    /// <summary>
    /// Implements the US DoD 5220.22-M (ECE) data sanitisation algorithm. Overwrites files 7 times. This method is considered highly secure and can
    /// be used to erase files that contain highly sensitive information.
    /// </summary>
    DoDSensitive = 2
}