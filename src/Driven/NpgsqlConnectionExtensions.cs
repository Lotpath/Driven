using Npgsql;

namespace Driven
{
    public static class NpgsqlConnectionExtensions
    {
        public static NpgsqlCommand CreateCommand(this NpgsqlConnection connection, string commandText, params object[] args)
        {
            var command = connection.CreateCommand();

            command.CommandText = commandText;

            foreach (var arg in args)
            {
                var p = command.CreateParameter();
                p.ParameterName = string.Format("@{0}", command.Parameters.Count);
                p.Value = arg;
                command.Parameters.Add(p);
            }

            return command;
        }
    }
}