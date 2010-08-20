namespace FakeItEasy.Core
{
    public interface IFakeObjectCallRuleWithDescription
        : IFakeObjectCallRule
    {
        /// <summary>
        /// Gets a description of calls the rule is applicable to.
        /// </summary>
        string DescriptionOfValidCall { get; }
    }
}
