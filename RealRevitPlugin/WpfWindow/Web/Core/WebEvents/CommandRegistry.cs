using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RealRevitPlugin.WpfWindow.Web.Core.WebEvents
{
    public class CommandRegistry
    {
        private readonly Dictionary<string, Func<Dictionary<string, JToken>, Task<string>>> _commands = new Dictionary<string, Func<Dictionary<string, JToken>, Task<string>>>();
        private readonly JsonSerializerSettings _jsonSettings;

        public CommandRegistry(JsonSerializerSettings jsonSettings)
        {
            _jsonSettings = jsonSettings;
        }

        /// <summary>
        /// Clears all registered commands.
        /// </summary>
        public void ClearCommands()
        {
            _commands.Clear();
        }

        public void RegisterCommand<TArgs, TResponse>(string commandName, Func<TArgs, Task<Result<TResponse>>> handler)
            where TArgs : class
        {
            _commands[commandName] = async (args) =>
            {
                Console.WriteLine($"Starting handler for command {commandName}");
                var argsJson = JsonConvert.SerializeObject(args);
                Console.WriteLine($"Command {commandName} args: {argsJson}");
                TArgs typedArgs;
                try
                {
                    typedArgs = JsonConvert.DeserializeObject<TArgs>(argsJson, _jsonSettings) ??
                        throw new ArgumentException($"Failed to deserialize arguments for command {commandName}");
                    Console.WriteLine($"Command {commandName} typedArgs: {JsonConvert.SerializeObject(typedArgs)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization error for {commandName}: {ex}");
                    var expectedType = typeof(TArgs);
                    var properties = expectedType.GetProperties();
                    var expectedStructure = string.Join(", ", properties.Select(p => $"{p.Name}: {p.PropertyType.Name}"));
                    throw new CommandExecutionException(
                        $"Invalid arguments for command {commandName}: {argsJson}. Expected structure: {{ {expectedStructure} }}",
                        ex);
                }

                try
                {
                    Console.WriteLine($"Executing handler for {commandName}");
                    var result = await handler(typedArgs);
                    Console.WriteLine($"Handler completed for {commandName}. Result: {result}");
                    var output = JsonConvert.SerializeObject(result.Output, _jsonSettings);
                    //Console.WriteLine($"Serialized output for {commandName}: {output}");
                    return output;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Handler error for {commandName}: {ex}");
                    throw new CommandExecutionException($"Error executing command {commandName}: {ex.Message}", ex);
                }
            };
        }

        public async Task<CommandResponse> HandleCommand(string input)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<CommandRequest>(input, _jsonSettings);
                if (request == null)
                {
                    return new CommandResponse { Success = false, Error = "Invalid request format", Id = null };
                }

                if (!_commands.TryGetValue(request.Command, out var handler))
                {
                    return new CommandResponse { Success = false, Error = $"Command '{request.Command}' not found", Id = request.Id };
                }
                var result = await handler(request.Args);
                return new CommandResponse { Success = true, Data = result, Id = request.Id };
            }
            catch (CommandExecutionException ex)
            {
                return new CommandResponse { Success = false, Error = ex.Message, Id = null };
            }
            catch (Exception ex)
            {
                return new CommandResponse { Success = false, Error = ex.Message, Id = null };
            }
        }
    }
}
