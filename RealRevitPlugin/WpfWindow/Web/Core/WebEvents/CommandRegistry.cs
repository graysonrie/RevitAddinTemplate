using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow.Web.Core.WebEvents {
    public class CommandRegistry {
        private readonly Dictionary<string, Func<Dictionary<string, JsonElement>, Task<string>>> _commands = new Dictionary<string, Func<Dictionary<string, JsonElement>, Task<string>>>();
        private readonly JsonSerializerOptions _jsonOptions;

        public CommandRegistry(JsonSerializerOptions jsonOptions) {
            _jsonOptions = jsonOptions;
        }

        /// <summary>
        /// Clears all registered commands.
        /// </summary>
        public void ClearCommands() {
            _commands.Clear();
        }

        public void RegisterCommand<TArgs, TResponse>(string commandName, Func<TArgs, Task<Result<TResponse>>> handler)
            where TArgs : class {
            _commands[commandName] = async (args) => {
                Console.WriteLine($"Starting handler for command {commandName}");
                var argsJson = JsonSerializer.Serialize(args);
                Console.WriteLine($"Command {commandName} args: {argsJson}");
                TArgs typedArgs;
                try {
                    typedArgs = JsonSerializer.Deserialize<TArgs>(argsJson, _jsonOptions) ??
                        throw new ArgumentException($"Failed to deserialize arguments for command {commandName}");
                    Console.WriteLine($"Command {commandName} typedArgs: {JsonSerializer.Serialize(typedArgs)}");
                }
                catch (Exception ex) {
                    Console.WriteLine($"Deserialization error for {commandName}: {ex}");
                    var expectedType = typeof(TArgs);
                    var properties = expectedType.GetProperties();
                    var expectedStructure = string.Join(", ", properties.Select(p => $"{p.Name}: {p.PropertyType.Name}"));
                    throw new CommandExecutionException(
                        $"Invalid arguments for command {commandName}: {argsJson}. Expected structure: {{ {expectedStructure} }}",
                        ex);
                }

                try {
                    Console.WriteLine($"Executing handler for {commandName}");
                    var result = await handler(typedArgs);
                    Console.WriteLine($"Handler completed for {commandName}. Result: {result}");
                    var output = JsonSerializer.Serialize(result.Output, _jsonOptions);
                    //Console.WriteLine($"Serialized output for {commandName}: {output}");
                    return output;
                }
                catch (Exception ex) {
                    Console.WriteLine($"Handler error for {commandName}: {ex}");
                    throw new CommandExecutionException($"Error executing command {commandName}: {ex.Message}", ex);
                }
            };
        }
        public async Task<CommandResponse> HandleCommand(string input) {
            try {
                var request = JsonSerializer.Deserialize<CommandRequest>(input, _jsonOptions);
                if (request == null) {
                    return new CommandResponse { Success = false, Error = "Invalid request format", Id = null };
                }

                if (!_commands.TryGetValue(request.Command, out var handler)) {
                    return new CommandResponse { Success = false, Error = $"Command '{request.Command}' not found", Id = request.Id };
                }
                var result = await handler(request.Args);
                return new CommandResponse { Success = true, Data = result, Id = request.Id };
            }
            catch (CommandExecutionException ex) {
                return new CommandResponse { Success = false, Error = ex.Message, Id = null };
            }
            catch (Exception ex) {
                return new CommandResponse { Success = false, Error = ex.Message, Id = null };
            }
        }
    }

}
