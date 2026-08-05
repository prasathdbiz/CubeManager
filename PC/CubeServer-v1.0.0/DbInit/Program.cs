using Microsoft.Data.SqlClient;
using System.Text;

var options = ParseArgs(args);

var server = GetOption(options, "server", "localhost,1433");
var database = GetOption(options, "database", "CubeMgr");
var trusted = GetFlag(options, "trusted") || GetFlag(options, "integrated");
var user = GetOption(options, "user", null);
var password = GetOption(options, "password", null);
var ignoreSeedDuplicates = GetFlag(options, "ignore-seed-duplicates") || GetFlag(options, "idempotent");

if (!trusted && (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password)))
{
    Console.Error.WriteLine("Provide --trusted or both --user and --password.");
    return 2;
}

var schemaPath = GetOption(options, "schema", null) ?? FindSqlFile("schema.sql");
var seedPath = GetOption(options, "seed", null) ?? FindSqlFile("seed_data.sql");

if (schemaPath == null || seedPath == null)
{
    Console.Error.WriteLine("Unable to locate schema.sql or seed_data.sql. Provide --schema and --seed paths explicitly.");
    return 3;
}

try
{
    await EnsureDatabaseExistsAsync(server, database, trusted, user, password);
    await ExecuteSqlFileAsync(server, database, trusted, user, password, schemaPath);
    await ExecuteSqlFileAsync(server, database, trusted, user, password, seedPath, ignoreDuplicateKeys: ignoreSeedDuplicates);

    Console.WriteLine("Database initialized.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.ToString());
    return 1;
}

static Dictionary<string, string> ParseArgs(string[] args)
{
    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    for (var i = 0; i < args.Length; i++)
    {
        var a = args[i] ?? string.Empty;
        if (!a.StartsWith("--", StringComparison.Ordinal))
        {
            continue;
        }

        var key = a.Substring(2);
        if (string.IsNullOrWhiteSpace(key))
        {
            continue;
        }

        if (i + 1 < args.Length && !(args[i + 1] ?? string.Empty).StartsWith("--", StringComparison.Ordinal))
        {
            dict[key] = args[i + 1];
            i++;
        }
        else
        {
            dict[key] = "true";
        }
    }

    return dict;
}

static bool GetFlag(Dictionary<string, string> options, string name)
{
    if (!options.TryGetValue(name, out var v))
    {
        return false;
    }

    return string.Equals(v, "true", StringComparison.OrdinalIgnoreCase)
        || string.Equals(v, "1", StringComparison.OrdinalIgnoreCase)
        || string.Equals(v, "yes", StringComparison.OrdinalIgnoreCase);
}

static string GetOption(Dictionary<string, string> options, string name, string defaultValue)
{
    if (options.TryGetValue(name, out var v) && !string.IsNullOrWhiteSpace(v))
    {
        return v;
    }

    return defaultValue;
}

static string FindSqlFile(string fileName)
{
    var cwd = Directory.GetCurrentDirectory();
    var candidates = new[]
    {
        Path.Combine(cwd, "CubeServer", "Data", fileName),
        Path.Combine(cwd, "PC", "CubeServer-v1.0.0", "CubeServer", "Data", fileName),
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CubeServer", "Data", fileName)),
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "CubeServer", "Data", fileName))
    };

    foreach (var c in candidates)
    {
        if (File.Exists(c))
        {
            return c;
        }
    }

    return null;
}

static string BuildConnectionString(string server, string database, bool trusted, string user, string password)
{
    var builder = new SqlConnectionStringBuilder
    {
        DataSource = server,
        InitialCatalog = database,
        TrustServerCertificate = true,
        Encrypt = false
    };

    if (trusted)
    {
        builder.IntegratedSecurity = true;
    }
    else
    {
        builder.UserID = user;
        builder.Password = password;
    }

    return builder.ConnectionString;
}

static async Task EnsureDatabaseExistsAsync(string server, string database, bool trusted, string user, string password)
{
    await using var conn = new SqlConnection(BuildConnectionString(server, "master", trusted, user, password));
    await conn.OpenAsync();

    var cmdText = $@"
IF DB_ID(N'{database.Replace("'", "''")}') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [{database.Replace("]", "]]")}]')
END";

    await using var cmd = new SqlCommand(cmdText, conn) { CommandTimeout = 120 };
    await cmd.ExecuteNonQueryAsync();
}

static async Task ExecuteSqlFileAsync(string server, string database, bool trusted, string user, string password, string filePath, bool ignoreDuplicateKeys = false)
{
    var sql = await File.ReadAllTextAsync(filePath);
    var batches = SplitSqlBatches(sql);

    await using var conn = new SqlConnection(BuildConnectionString(server, database, trusted, user, password));
    await conn.OpenAsync();

    foreach (var batch in batches)
    {
        if (string.IsNullOrWhiteSpace(batch))
        {
            continue;
        }

        try
        {
            await using var cmd = new SqlCommand(batch, conn) { CommandTimeout = 120 };
            await cmd.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ignoreDuplicateKeys && (ex.Number == 2627 || ex.Number == 2601))
        {
        }
    }
}

static IEnumerable<string> SplitSqlBatches(string sql)
{
    if (sql == null)
    {
        yield break;
    }

    var sb = new StringBuilder();
    var lines = sql.Replace("\r\n", "\n").Split('\n');

    foreach (var line in lines)
    {
        if (string.Equals(line.Trim(), "GO", StringComparison.OrdinalIgnoreCase))
        {
            yield return sb.ToString();
            sb.Clear();
            continue;
        }

        sb.AppendLine(line);
    }

    if (sb.Length > 0)
    {
        yield return sb.ToString();
    }
}
