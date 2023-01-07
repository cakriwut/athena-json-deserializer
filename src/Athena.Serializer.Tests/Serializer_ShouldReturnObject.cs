namespace Athena.Serializer.Tests;
using Xunit;
using Athena.Serializer;
using System.Reflection;

public class Serializer_ShouldReturnObject
{
    private readonly string AthenaJSONString = @"
    { location = Singapore, 
      customer = John Doe,
      address = PO Box 123 (Toa Payoh),
      discount = [ 10, 20, 30], 
      items = [ { name = 'Apple', price = 1.20, quantity = 10 }, 
              { name = 'Orange', price = 1.50, quantity = 5 }, 
              { name = 'Banana', price = 1.00, quantity = 20 } ]
    ";

    private readonly string AthenaJSONString2 = @"
    [{ name = 'Apple', price = 1.20, quantity = 10 ,discount = [ 10, 20, 30] , delivery = {location = Singapore, customer = John Doe,address = PO Box 123 (Toa Payoh) }}, 
     { name = 'Orange', price = 1.50, quantity = 5 , discount = [ 10, 20, 30], delivery = {location = Singapore, customer = John Doe,address = PO Box 123 (Toa Payoh) }}, 
     { name = 'Banana', price = 1.00, quantity = 20 , discount = [ 10, 20, 30] , delivery = {location = Singapore, customer = John Doe,address = PO Box 123 (Toa Payoh) }} ]";
    
    [Fact(DisplayName = "First level contains five fields")]
    public void ShouldReturn_FirstLevel_ContainsFiveFields()
    {
        var deserialized = TestPrivate.StaticMethod<Dictionary<string, object>>(typeof(AthenaJsonStringExtensions), "GetObject", new object[] { AthenaJSONString });
        Assert.Equal(5, deserialized.Count());
    }


    [Fact(DisplayName = "First level contains three fields")]
    public void ShouldReturn_FirstLevel_ContainsThreeObjects()
    {
        var deserialized = TestPrivate.StaticMethod<List<object>>(typeof(AthenaJsonStringExtensions), "GetArray", new object[] { AthenaJSONString2 });
        Assert.Equal(3, deserialized.Count());
    }
}


public static class TestPrivate
{
    public static T StaticMethod<T>(Type classType, string methodName, object[] callParams)
    {
        var methodList = classType
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static);

        if (methodList is null || !methodList.Any())
            throw new EntryPointNotFoundException();

        var method = methodList.First(x => x.Name == methodName && !x.IsPublic && x.GetParameters().Length == callParams.Length);

        var output = (T)method.Invoke(null, callParams);

        return output;
    }

    public static T InstanceMethod<T>(object instance, string methodName, object[] callParams)
    {
        var classType = instance.GetType();
        var methodList = classType
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodList is null || !methodList.Any())
            throw new EntryPointNotFoundException();

        var method = methodList.First(x => x.Name == methodName && !x.IsPublic && x.GetParameters().Length == callParams.Length);

        var output = (T)method.Invoke(instance, callParams);

        return output;
    }
}