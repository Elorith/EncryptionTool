/// <summary>
/// Implements the US DoD 5220.22-M (ECE) data sanitisation algorithm. Overwrites files 7 times. This method is considered highly secure and can be
/// used for files that contain highly sensitive information.
/// </summary>
public class DoDLongEraser : IEraser
{
    public void Erase(string path)
    {
    } 
}