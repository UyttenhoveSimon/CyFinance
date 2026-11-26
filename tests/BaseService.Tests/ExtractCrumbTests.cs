using TUnit;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.BaseService;

public class ExtractCrumbTests
{
    // ExtractCrumb is protected: expose it through a minimal subclass.
    private sealed class CrumbReader() : CyFinance.BaseService(new HttpClient())
    {
        public string? Read(string html) => ExtractCrumb(html);
    }

    private static readonly CrumbReader Reader = new();

    [Test]
    public async Task ReadsTheCrumbStoreShape()
    {
        var crumb = Reader.Read("""<script>{"CrumbStore": { "crumb": "abc123" }}</script>""");
        await Assert.That(crumb).IsEqualTo("abc123");
    }

    [Test]
    public async Task ReadsABareCrumbField()
    {
        var crumb = Reader.Read("""{"user":{"crumb":"xyz789","id":1}}""");
        await Assert.That(crumb).IsEqualTo("xyz789");
    }

    [Test]
    public async Task MatchesKeysCaseInsensitively()
    {
        var crumb = Reader.Read("""{"CRUMB": "Up9er"}""");
        await Assert.That(crumb).IsEqualTo("Up9er");
    }

    [Test]
    public async Task UnescapesUnicodeSequences()
    {
        var crumb = Reader.Read("""{"crumb":"a/bA"}""");
        await Assert.That(crumb).IsEqualTo("a/bA");
    }

    [Test]
    public async Task ReturnsNullWithoutACrumb()
    {
        var crumb = Reader.Read("<html><body>no token here</body></html>");
        await Assert.That(crumb).IsNull();
    }
}
