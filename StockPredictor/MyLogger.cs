namespace StockPredictor;

public class MyLogger : IMyLogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
        // System.Diagnostics.Debug.WriteLine(message);
    }
}