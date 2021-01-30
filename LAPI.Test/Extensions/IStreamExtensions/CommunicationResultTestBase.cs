using System.Threading;
using LAPI.Contracts;
using LAPI.Model;

namespace LAPI.Test.Extensions.IStreamExtensions
{
    public interface ITokenTestBase
    {
        CancellationToken Token { get; }
    }
    public interface IClientTestBase : ITokenTestBase
    {
        IStream Client { get; set; }
    }
    public interface IServerTestBase : ITokenTestBase
    {
        IStream Server { get; set; }
    }
    public interface IStreamTestBase : IClientTestBase, IServerTestBase { }
    public interface ICommunicationResultTestBase
    {
        CommunicationResult Result { get; set; }
    }
    public interface IClientResultTestBase : ICommunicationResultTestBase, IClientTestBase { }
    public interface IServerResultTestBase : ICommunicationResultTestBase, IServerTestBase { }
    public interface IStreamResultTestBase : IClientResultTestBase, IServerResultTestBase, IStreamTestBase { }

    public interface ICommunicationResultTestBase<TResult> : ICommunicationResultTestBase
    {
        CommunicationResult<TResult> Result { get; set; }
    }
    public interface IClientResultTestBase<TResult> : ICommunicationResultTestBase<TResult>, IClientTestBase { }
    public interface IServerResultTestBase<TResult> : ICommunicationResultTestBase<TResult>, IServerTestBase { }
    public interface IStreamResultTestBase<TResult> : IClientResultTestBase<TResult>, IServerResultTestBase<TResult>, IStreamTestBase { }

    public abstract class CommunicationResultTestBase : GivenWhenThen<CommunicationResultTestBase>, IStreamResultTestBase
    {
        protected override CommunicationResultTestBase Base => this;

        public IStream Server { get; set; }
        public IStream Client { get; set; }

        public CancellationToken Token { get; } = CancellationToken.None;
        public CommunicationResult Result { get; set; }
    }
    public abstract class CommunicationResultTestBase<TResult> : GivenWhenThen<CommunicationResultTestBase<TResult>>, IStreamResultTestBase<TResult>
    {
        protected override CommunicationResultTestBase<TResult> Base => this;

        public IStream Server { get; set; }
        public IStream Client { get; set; }

        public CancellationToken Token { get; } = CancellationToken.None;
        public CommunicationResult<TResult> Result { get; set; }
        CommunicationResult ICommunicationResultTestBase.Result
        {
            get => Result;
            set { }
        }
    }
}