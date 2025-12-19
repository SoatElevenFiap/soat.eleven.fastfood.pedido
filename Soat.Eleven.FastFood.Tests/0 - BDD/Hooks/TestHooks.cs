using TechTalk.SpecFlow;

namespace Soat.Eleven.FastFood.Tests.BDD.Hooks;

[Binding]
public class TestHooks
{
    [BeforeScenario]
    public void BeforeScenario()
    {
        // Configurações iniciais antes de cada cenário
    }

    [AfterScenario]
    public void AfterScenario()
    {
        // Limpeza após cada cenário
    }
}
