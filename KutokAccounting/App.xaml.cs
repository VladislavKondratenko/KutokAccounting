using KutokAccounting.DataProvider;
using KutokAccounting.Logging;
using KutokAccounting.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KutokAccounting;

public partial class App : Application
{
	private readonly KutokDbContext _dbContext;
	private readonly ILogger<App> _logger;
	private readonly IFileLoggerProcessor _processor;
	private CancellationTokenSource _cancellationTokenSource = new();
	private Task _loggingTask;

	public App(KutokDbContext dbContext,
		ILogger<App> logger,
		IFileLoggerProcessor processor)
	{
		_dbContext = dbContext;
		_logger = logger;
		_processor = processor;
	}

	protected override void OnStart()
	{
		InitializeComponent();
		
		_dbContext.Database.MigrateAsync(_cancellationTokenSource.Token).ContinueWith(t =>
		{
			if (t.IsCompletedSuccessfully)
			{
				_logger.LogInformation("Migration applied successfully.");
			}
			else
			{
				_logger.LogError(t.Exception, "Migration failed.");
			}
		});

		LogFilesCleaner.CleanAsync(_cancellationTokenSource.Token).ContinueWith(t =>
		{
			_logger.LogInformation("Old logs was successfully cleaned.");
		});

		_loggingTask = _processor.ProcessAsync(_cancellationTokenSource.Token);
	}

	protected override void OnSleep()
	{
		_cancellationTokenSource.Cancel();
	}

	protected override void OnResume()
	{
		if (_loggingTask.IsCompleted)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_loggingTask = _processor.ProcessAsync(_cancellationTokenSource.Token);
		}
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage())
		{
			Title = "Kutok"
		};
	}
}