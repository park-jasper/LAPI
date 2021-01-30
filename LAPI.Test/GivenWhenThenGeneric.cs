namespace LAPI.Test
{
    public interface IGiven<in TBase>
    {
        void Given(TBase tbase);
    }
	public interface IGiven<in TBase, in TParameter>
    {
        void Given(TBase tbase, TParameter parameter);
	}
    public interface IWhen<in TBase>
    {
        void When(TBase tbase);
	}
    public interface IWhen<in TBase, in TParameter>
    {
        void When(TBase tbase, TParameter parameter);
	}
    public interface IThen<in TBase>
    {
        void Then(TBase tbase);
	}
    public interface IThen<in TBase, in TParameter>
    {
        void Then(TBase tbase, TParameter parameter);
    }

	public abstract class GivenWhenThen<TBase>
	{
		protected abstract TBase Base { get; }
		protected void Given<TGiven>()
			where TGiven : IGiven<TBase>, new()
		{
			new TGiven().Given(Base);
		}
		protected void When<TWhen>()
			where TWhen : IWhen<TBase>, new()
		{
			new TWhen().When(Base);
		}
		protected void Then<TThen>()
			where TThen : IThen<TBase>, new()
		{
			new TThen().Then(Base);
		}
		protected void Given<TGiven1, TParameter>(TParameter parameter)
			where TGiven1 : IGiven<TBase, TParameter>, new()
		{
			new TGiven1().Given(Base, parameter);
		}
		protected void When<TWhen1, TParameter>(TParameter parameter)
			where TWhen1 : IWhen<TBase, TParameter>, new()
		{
			new TWhen1().When(Base, parameter);
		}
		protected void Then<TThen1, TParameter>(TParameter parameter)
			where TThen1 : IThen<TBase, TParameter>, new()
		{
			new TThen1().Then(Base, parameter);
		}
	}
}