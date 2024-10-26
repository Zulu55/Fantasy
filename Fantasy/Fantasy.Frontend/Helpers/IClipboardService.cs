namespace Fantasy.Frontend.Helpers;

public interface IClipboardService
{
    Task CopyToClipboardAsync(string text);
}