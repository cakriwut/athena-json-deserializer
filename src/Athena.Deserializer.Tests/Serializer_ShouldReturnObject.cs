namespace Athena.Deserializer.Tests;
using Xunit;
using Athena.Deserializer;
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
    } ";

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

    [Fact(DisplayName = "Should return Dictionary Objects")]
    public void ShouldReturn_DictionaryObjects()
    {
        Dictionary<string,object> deserialized = AthenaJSONString.ToJsonObject<Dictionary<string,object>>();
             
        Assert.Equal("Singapore", deserialized["location"].ToString());
        Assert.Equal("John Doe", deserialized["customer"].ToString());
    }

    [Fact(DisplayName = "Should return Objects")]
    public void ShouldReturn_Objects()
    {
        SchemaJson1 deserialized = AthenaJSONString.ToJsonObject<SchemaJson1>();
        var expected = new SchemaJson1
        {
            Location = "Singapore",
            Customer = "John Doe",
            Address = "PO Box 123 (Toa Payoh)"
        };


        Assert.Equal(expected, deserialized);
    }

    [Fact(DisplayName = "Should return Array")]
    public void ShouldReturn_Array()
    {
        var deserialized = AthenaJSONString.ToJsonObject<SchemaJson2>();
        int[] expected = new int[] { 10, 20, 30 };

        Assert.Equal(3, deserialized.Discount.Count());
        Assert.Equal(expected, deserialized.Discount);
    }
}


public record SchemaJson1
{
    public string Location { get; set; }
    public string Customer { get; set; }
    public string Address { get; set; }
}


public record SchemaJson2
{
    public int[] Discount { get; set; }
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