using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LAPI.Test
{
    public abstract class TestSpecs
    {
        [TestInitialize]
        public void Init()
        {
            Given();
            When();
        }

        public virtual void Given() {}
        public virtual void When() {}
    }

    public abstract class TestSpecsAsync
    {
        public delegate Task AsyncAction();
        [TestInitialize]
        public async Task Init()
        {
            await Given();
            await When();
        }

        public virtual Task Given()
        {
            return Task.CompletedTask;
        }

        public virtual Task When()
        {
            return Task.CompletedTask;
        }


        protected Task DoParallel(params AsyncAction[] actions)
        {
            var tasks = actions.Select(a => a()).ToList();
            return Task.WhenAll(tasks);
        }
    }
}