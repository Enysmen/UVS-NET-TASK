using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DatabaseSchema.Infrastructure
{
    public class DatabaseInitializer
    {
        private readonly string _password;
        private readonly string _database;
        private readonly string _port;
        private readonly string _schemaLocation;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IConfiguration config, ILogger<DatabaseInitializer> logger)
        {
            
            _password = config["UvsTaskPassword"] ?? throw new InvalidOperationException("Missing UvsTaskPassword");
            _database = config["UvsTaskDatabase"] ?? throw new InvalidOperationException("Missing UvsTaskDatabase");
            _port = config["UvsTaskPort"] ?? throw new InvalidOperationException("Missing UvsTaskPort");
            _schemaLocation = config["UvsTaskSchemaLocation"] ?? throw new InvalidOperationException("Missing UvsTaskSchemaLocation");
            _logger = logger;
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Waiting for database to start...");
            await WaitForConnection(ct);

            _logger.LogInformation("Creating database '{Database}'...", _database);
            await CreateDatabaseAsync(ct);

            _logger.LogInformation("Importing schema from {File}...", _schemaLocation);
            await ImportSchemaAsync(ct);

            _logger.LogInformation("Database initialization complete.");
        }

        private async Task WaitForConnection(CancellationToken ct)
        {
            var start = DateTime.UtcNow;
            Exception? lastEx = null;
            while (DateTime.UtcNow - start < TimeSpan.FromSeconds(30))
            {
                try
                {
                    var connString = $"Host=localhost;Port={_port};Username=postgres;Password={_password};";
                    using var cn = new NpgsqlConnection(connString);
                    await cn.OpenAsync(ct);
                    return;
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    _logger.LogDebug("Connection attempt failed: {Error}", ex.Message);
                    await Task.Delay(1000, ct);
                }
            }
            throw new InvalidOperationException("Cannot connect to DB", lastEx);
        }

        private  async Task CreateDatabaseAsync(CancellationToken ct = default)
        {
            
            var connString = $"Host=localhost;Port={_port};Username=postgres;Password={_password};";
            await using var cn = new NpgsqlConnection(connString);
            await cn.OpenAsync(ct);

            
            await using var checkCmd = cn.CreateCommand();
            checkCmd.CommandText = @"
                SELECT 1 
                FROM pg_database 
                WHERE datname = @dbName
                ";
            checkCmd.Parameters.AddWithValue("dbName", _database);
            var exists = await checkCmd.ExecuteScalarAsync(ct) != null;  

            if (exists)
            {
                Console.WriteLine($"Database '{_database}' already exists, skipping creation.");
            }
            else
            {
                
                await using var createCmd = cn.CreateCommand();
                createCmd.CommandText = $@"CREATE DATABASE ""{_database}"";";
                await createCmd.ExecuteNonQueryAsync(ct);
                Console.WriteLine($"Database '{_database}' created successfully.");
            }
        }

        private async Task ImportSchemaAsync(CancellationToken ct)
        {
            
            var baseDir = AppContext.BaseDirectory;
            var schemaPath = Path.Combine(baseDir, "dbSchema.sql");

            if (!File.Exists(schemaPath))
            {
                throw new FileNotFoundException($"Schema file not found: {schemaPath}");
            }

            var sql = await File.ReadAllTextAsync(schemaPath, ct);
            await using var cn = new NpgsqlConnection( $"Host=localhost;Port={_port};Username=postgres;Password={_password};Database={_database};");
            await cn.OpenAsync(ct);
            await using var cmd = cn.CreateCommand();
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
