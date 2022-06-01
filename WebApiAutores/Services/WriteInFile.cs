namespace WebApiAutores.Services
{
    public class WriteInFile : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string archiveName = "Archive 1.txt";
        private Timer timer;

        public WriteInFile(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Write("Initialized process");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Write("Finalized process");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Write("Process in execution: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        private void Write (string message)
        {
            var route = $@"{env.ContentRootPath}\wwwroot\{archiveName}";

            using(StreamWriter writer = new StreamWriter(route, append: true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
