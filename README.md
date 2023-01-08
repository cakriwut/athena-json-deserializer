# Athena JSON Deserializer

This library provides a JSON deserializer for the [Athena](https://docs.aws.amazon.com/athena/latest/ug/what-is.html). At the larger perspective, it decodes JSON having equal sign instead of colon into proper JSON.

## What it does?
Consider the following JSON having equal sign:
```json
{
  a = Singapore,
  b = Some address in Singapore (Postal Code 123456)
}
```

The library will convert it into proper JSON:
```json
{
  "a": "Singapore",
  "b": "Some address in Singapore (Postal Code 123456)"
}
```

## Getting Started

1. Install Package  
    Using Package Manager:
    ```powershell
    PM> Install-Package AthenaJsonDeserializer
    ```
    Using .NET CLI:
    ```dotnetcli
    > dotnet add-package AthenaJsonDeserializer
    ```
1. Use the `AthenaJsonDeserializer` class to deserialize JSON string:
    ```csharp
    var json = @"{
      a = Singapore,
      b = Some address in Singapore (Postal Code 123456)
    }";
    var deserializedJson = AthenaJsonDeserializer.Deserialize(json);
    ```
    or use the `AthenaJsonDeserializer` extension method:
    ```csharp
    var json = @"{
      a = Singapore,
      b = Some address in Singapore (Postal Code 123456)
    }";
    var deserializedJson = json.Deserialize();
    ```