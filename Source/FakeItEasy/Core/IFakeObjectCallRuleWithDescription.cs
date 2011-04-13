namespace FakeItEasy.Core
{
    /// <summary>
    /// Represents a call rule that has a description of the calls the
    /// rule is applicable to.
    /// </summary>
    public interface IFakeObjectCallRuleWithDescription
        : IFakeObjectCallRule
    {
        /// <summary>
        /// Writes a description of calls the rule is applicable to.
        /// </summary>
        /// <param name="writer"></param>
        void WriteDescriptionOfValidCall(IOutputWriter writer);
    }
}