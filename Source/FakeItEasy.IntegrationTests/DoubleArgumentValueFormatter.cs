namespace FakeItEasy.IntegrationTests
{
    public class DoubleValueFormatter : ArgumentValueFormatter<double>
    {
        protected override string GetStringValue(double argumentValue)
        {
            return "[" + argumentValue + "]";
        }
    }
}
