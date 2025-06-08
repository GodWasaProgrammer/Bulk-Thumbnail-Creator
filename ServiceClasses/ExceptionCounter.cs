namespace BulkThumbnailCreator.ServiceClasses;
public static class ExceptionCounter
{
    private static int _count;
    public static int Count => _count;

    public static void Increment() => Interlocked.Increment(ref _count);
}
