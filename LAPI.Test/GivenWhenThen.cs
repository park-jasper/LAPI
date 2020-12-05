using System;
using System.Threading.Tasks;
using MoreLinq;

namespace LAPI.Test
{
    public abstract class GivenWhenThen
    {
        private static void Invoke(Action action) => action?.Invoke();
        protected void Given(params Action[] givenActions)
        {
            givenActions.ForEach(Invoke);
        }

        protected void When(params Action[] whenActions)
        {
            whenActions.ForEach(Invoke);
        }

        protected void Then(params Action[] thenActions)
        {
            thenActions.ForEach(Invoke);
        }
    }

    public abstract class GivenWhenThenAsync
    {
        public delegate Task AsyncAction();


        protected async Task Given(params AsyncAction[] givenAsyncActions)
        {
            foreach (var asyncAction in givenAsyncActions)
            {
                await asyncAction();
            }
        }
        protected async Task When(params AsyncAction[] whenAsyncActions)
        {
            foreach (var asyncAction in whenAsyncActions)
            {
                await asyncAction();
            }
        }
        protected async Task Then(params AsyncAction[] thenAsyncActions)
        {
            foreach (var asyncAction in thenAsyncActions)
            {
                await asyncAction();
            }
        }
    }
}