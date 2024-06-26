# Json2CSharpConverter
Converts JSON to C# code, where the result C# code uses System.Text.Json to construct the same JSON file programmatically

This is an extremely simple utility that only has just about 200 lines of code, and converts JSON to C#. For example:

**input**
```json
{
  "properties": {
    "name": "User",
    "created": "26.06.2024"
  },
  "credentials": {
    "password": "An example password"
  }
}
```

**output**
```cs
using var ms = new MemoryStream();
using var jsonWriter = new Utf8JsonWriter(ms);

jsonWriter.WriteStartObject();
jsonWriter.WritePropertyName("properties");
jsonWriter.WriteStartObject();
jsonWriter.WriteString("name", "User");
jsonWriter.WriteString("created", "26.06.2024");
jsonWriter.WriteEndObject();
jsonWriter.WritePropertyName("credentials");
jsonWriter.WriteStartObject();
jsonWriter.WriteString("password", "An example password");
jsonWriter.WriteEndObject();
jsonWriter.WriteEndObject();

jsonWriter.Flush();
```

Before using the library, you can try it using Json2CSharpConverter.exe, simply pass the path to
the json file as the first argument and it will print the C# output directly.
