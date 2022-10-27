using RR.Dynamics365.SpecFlow.Steps;

namespace RR.Dynamics365.SpecFlow.UnitTests.Steps
{
    public class GenericStepsTests
    {
        [Fact]
        public void Should_not_fail()
        {
            // Arrange
            var steps = new GenericSteps();

            // Act
            // Assert
            steps.GivenExistingEntity("account", "Awersome property");
        }
    }
}
