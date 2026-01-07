namespace KutokAccounting.Logging;

internal class LogFileWriter : IDisposable, IAsyncDisposable
{
	private readonly FileStream _fileStream;
	private readonly StreamWriter _streamWriter;
	private readonly SemaphoreSlim _semaphoreSlim;

	public LogFileWriter()
	{
		if (Directory.Exists(FileLoggerConfiguration.LoggingDirectoryPath) is false)
		{
			Directory.CreateDirectory(FileLoggerConfiguration.LoggingDirectoryPath);
		}

		string combine = Path.Combine(FileLoggerConfiguration.LoggingDirectoryPath,
			$"{DateTime.Now:yyyy-MM-dd-HH.mm}.log");

		_semaphoreSlim = new SemaphoreSlim(1, 1);
		_fileStream = new FileStream(combine, FileMode.OpenOrCreate, FileAccess.Write);
		_streamWriter = new StreamWriter(_fileStream, leaveOpen: true);
	}

	public async ValueTask DisposeAsync()
	{
		await _fileStream.DisposeAsync();
		await _streamWriter.DisposeAsync();
		_semaphoreSlim.Dispose();
	}

	public void Dispose()
	{
		_fileStream.Dispose();
		_streamWriter.Dispose();
		_semaphoreSlim.Dispose();
	}

	public async ValueTask WriteAsync(string log, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			await _streamWriter.WriteAsync(log.AsMemory(), cancellationToken);
			await _streamWriter.FlushAsync(cancellationToken);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}
}