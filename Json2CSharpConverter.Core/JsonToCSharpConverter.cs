using System.Text;
using System.Text.Json;

namespace Json2CSharpConverter.Core
{
    /// <summary>
    /// Converts JSON to C#.
    /// </summary>
    public class JsonToCSharpConverter
    {
        private readonly StringBuilder _builder;

        /// <summary>
        /// Gets &amp; sets the name of the variable that contains <see cref="Utf8JsonWriter"/>.
        /// </summary>
        public string Utf8WriterVariableName { get; set; } = "writer";

        /// <summary>
        /// Should the following code be emitted?:
        /// <code>
        ///  using var ms = new MemoryStream();
        ///  using var writer = new Utf8JsonWriter(ms);
        /// </code>
        /// </summary>
        public bool EmitSetup { get; set; } = true;

        /// <summary>
        /// Should the following code be emitted?:
        /// <code>
        ///   writer.Flush();
        /// </code>
        /// </summary>
        public bool EmitFlush { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonToCSharpConverter"/> class.
        /// </summary>
        public JsonToCSharpConverter()
        {
            _builder = new StringBuilder();
        }

        /// <summary>
        /// Converts the given JSON document to C# code that uses System.Text.Json.
        /// </summary>
        /// <param name="document">Input JSON document.</param>
        /// <returns>A string that represents C# code.</returns>
        public string Convert(JsonDocument document)
        {
            _builder.Clear();
            AppendSetup();
            ConvertTopLevelObject(root: document.RootElement);
            AppendFlush();
            return _builder.ToString();
        }

        /// <summary>
        /// Converts the given JSON string to C# code that uses System.Text.Json.
        /// </summary>
        /// <param name="json">Input JSON string.</param>
        /// <returns>A string that represents C# code.</returns>
        public string Convert(string json) => Convert(JsonDocument.Parse(json));

        private void AppendFlush()
        {
            if (EmitFlush)
            {
                _builder.AppendLine();
                _builder.AppendLine($"{Utf8WriterVariableName}.Flush();");
            }
        }

        private void AppendSetup()
        {
            if (EmitSetup)
            {
                _builder.AppendLine($"using var ms = new MemoryStream();");
                _builder.AppendLine($"using var {Utf8WriterVariableName} = new Utf8JsonWriter(ms);");
                _builder.AppendLine();
            }
        }

        private void ConvertTopLevelObject(JsonElement root)
        {
            switch (root.ValueKind)
            {
                case JsonValueKind.Array:
                    ConvertArray(root);
                    break;
                case JsonValueKind.Object:
                    ConvertObject(root);
                    break;
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteNullValue();");
                    break;
                case JsonValueKind.String:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteStringValue(\"{root.GetString()}\");");
                    break;
                case JsonValueKind.Number:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteNumberValue({root.GetRawText()});");
                    break;
                case JsonValueKind.True:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteBooleanValue(true);");
                    break;
                case JsonValueKind.False:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteBooleanValue(false);");
                    break;
                default:
                    break;
            }
        }

        private void ConvertPropertyWrites(JsonProperty property)
        {
            string name = property.Name;
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.String:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteString(\"{name}\", \"{property.Value.GetString()}\");");
                    break;
                case JsonValueKind.Number:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteNumber(\"{name}\", {property.Value.GetRawText()});");
                    break;
                case JsonValueKind.True:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteBoolean(\"{name}\", true);");
                    break;
                case JsonValueKind.False:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteBoolean(\"{name}\", false);");
                    break;
                case JsonValueKind.Null:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteNull(\"{name}\");");
                    break;
                case JsonValueKind.Object:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WritePropertyName(\"{name}\");");
                    ConvertObject(property.Value);
                    break;
                case JsonValueKind.Array:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WritePropertyName(\"{name}\");");
                    ConvertArray(property.Value);
                    break;
                default:
                    break;
            }
        }

        private void ConvertObject(JsonElement element)
        {
            _builder.AppendLine($"{Utf8WriterVariableName}.WriteStartObject();");
            foreach (JsonProperty property in element.EnumerateObject())
            {
                ConvertPropertyWrites(property);
            }
            _builder.AppendLine($"{Utf8WriterVariableName}.WriteEndObject();");
        }

        private void ProcessArrayChild(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    ConvertObject(element);
                    break;
                case JsonValueKind.Array:
                    ConvertArray(element);
                    break;
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteNullValue();");
                    break;
                case JsonValueKind.String:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteStringValue({element.GetRawText()});");
                    break;
                case JsonValueKind.Number:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteNumberValue({element.GetRawText()});");
                    break;
                case JsonValueKind.True:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteBooleanValue(true);");
                    break;
                case JsonValueKind.False:
                    _builder.AppendLine($"{Utf8WriterVariableName}.WriteBooleanValue(false);");
                    break;
                default:
                    break;
            }
        }

        private void ConvertArray(JsonElement element)
        {
            _builder.AppendLine($"{Utf8WriterVariableName}.WriteStartArray();");
            foreach (JsonElement childElement in element.EnumerateArray())
            {
                ProcessArrayChild(childElement);
            }
            _builder.AppendLine($"{Utf8WriterVariableName}.WriteEndArray();");
        }
    }
}
