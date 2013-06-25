namespace FakeItEasy.IntegrationTests
{
    public class DoubleValueFormatter : ArgumentValueFormatter<double>
    {
        protected override string GetStringValue(double value)
        {
            return "[" + value + "]";
        }
    }
}
