namespace ViewModel
{
    internal interface IReaction
    {
        bool IsActive { get; }
        ScenarioViewModel Scenario { get; }

        void Check();
    }
}