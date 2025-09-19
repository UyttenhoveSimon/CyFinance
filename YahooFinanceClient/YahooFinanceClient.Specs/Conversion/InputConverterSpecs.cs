using TUnit.Core;
using TUnit.Assertions;
using YahooFinanceClient.Conversion;
using System.Threading.Tasks;
using System; // Assuming this is the namespace for InputConverter

namespace YahooFinanceClient.Specs.Conversion;

public class InputConverterTests
{
    // A single instance can be reused if the class is stateless.
    private readonly InputConverter _inputConverter = new();

    [Test]
    public async Task ConvertStringToDecimal_WithValidDecimalString_ReturnsCorrectDecimal()
    {
        // Arrange
        const string input = "9.3";
        const decimal expected = 9.3M;

        // Act
        var result = _inputConverter.ConvertStringToDecimal(input);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task ConvertStringToDate_WithValidDateString_ReturnsCorrectDateTime()
    {
        // Arrange
        const string input = "2/2/2017";
        var expected = new DateTime(2017, 2, 2);

        // Act
        var result = _inputConverter.ConvertStringToDate(input);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task CheckIfNotAvailable_WithAvailableString_ReturnsTheString()
    {
        // Arrange
        const string input = "2.78B";

        // Act
        var result = _inputConverter.CheckIfNotAvailable(input);

        // Assert
       await Assert.That(result).IsEqualTo(input);
    }

    [Arguments("+2.5%", 2.5)]
    [Arguments("-2.5%", -2.5)]
    [Test]
    public async Task ConvertStringToPercentDecimal_WithValidPercentString_ReturnsCorrectDecimal(string input, double expectedValue)
    {
        // Arrange
        var expected = (decimal)expectedValue;

        // Act
        var result = _inputConverter.ConvertStringToPercentDecimal(input);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Arguments("N/A")]
    [Arguments("N / A")]
    [Arguments("N/A\n")]
    [Arguments("N / A\n")]
    [Arguments("")]
    [Arguments(null)]
    [Test]
    public async Task ConvertStringToDecimal_WithNotAvailableOrEmptyStrings_ReturnsNull(string? input)
    {
        // Act
        var result = _inputConverter.ConvertStringToDecimal(input);

        // Assert
        await Assert.That(result).IsNull();
    }
    
    [Test]
    public async Task ConvertStringToDate_WithNotAvailableString_ReturnsNull()
    {
        // Act
        var result = _inputConverter.ConvertStringToDate("N/A");

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task ConvertStringToPercentDecimal_WithNotAvailableString_ReturnsNull()
    {
        // Act
        var result = _inputConverter.ConvertStringToPercentDecimal("N/A");

        // Assert
        await Assert.That(result).IsNull();
    }
    
    [Test]
    public async Task CheckIfNotAvailable_WithNotAvailableString_ReturnsNull()
    {
        // Act
        var result = _inputConverter.CheckIfNotAvailable("N/A");

        // Assert
        await Assert.That(result).IsNull();
    }
}