using System;
using System.Collections.Generic;
using AElf.Cryptography.ECDSA;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Moq;

namespace AElf.Contracts.MultiToken
{
    public class TokenContractTestBase
    {
        protected readonly Mock<ITokenContract> TokenContract;
        protected readonly TokenContractState State;
        protected readonly Address DefaultAddress;
        protected readonly Address User1Address;
        protected readonly Address User2Address;

        public TokenContractTestBase()
        {
            TokenContract = new Mock<ITokenContract>();
            State = new TokenContractState();
            
            // Initialize addresses
            DefaultAddress = GenerateAddress();
            User1Address = GenerateAddress();
            User2Address = GenerateAddress();

            // Set up contract state
            State.ChainId = 1;
            State.Owner = DefaultAddress;
            TokenContract.Setup(x => x.State).Returns(State);

            // Set up mock behavior
            TokenContract.Setup(x => x.Create(It.IsAny<TokenInfo>())).Callback<TokenInfo>(info =>
            {
                State.TokenInfos[info.Symbol] = info;
                State.Balances[info.Symbol] = new Dictionary<Address, long>
                {
                    [DefaultAddress] = info.TotalSupply,
                    [User1Address] = 0,
                    [User2Address] = 0
                };
                State.Allowances[info.Symbol] = new Dictionary<Address, Dictionary<Address, long>>
                {
                    [DefaultAddress] = new Dictionary<Address, long>(),
                    [User1Address] = new Dictionary<Address, long>(),
                    [User2Address] = new Dictionary<Address, long>()
                };
            });

            TokenContract.Setup(x => x.Issue(It.IsAny<IssueInput>())).Callback<IssueInput>(input =>
            {
                State.Balances[input.Symbol][DefaultAddress] -= input.Amount;
                if (!State.Balances[input.Symbol].ContainsKey(input.To))
                {
                    State.Balances[input.Symbol][input.To] = 0;
                }
                State.Balances[input.Symbol][input.To] += input.Amount;
            });

            TokenContract.Setup(x => x.Transfer(It.IsAny<TransferInput>())).Callback<TransferInput>(input =>
            {
                State.Balances[input.Symbol][User1Address] -= input.Amount;
                if (!State.Balances[input.Symbol].ContainsKey(input.To))
                {
                    State.Balances[input.Symbol][input.To] = 0;
                }
                State.Balances[input.Symbol][input.To] += input.Amount;
            });

            TokenContract.Setup(x => x.Approve(It.IsAny<ApproveInput>())).Callback<ApproveInput>(input =>
            {
                if (!State.Allowances[input.Symbol].ContainsKey(User1Address))
                {
                    State.Allowances[input.Symbol][User1Address] = new Dictionary<Address, long>();
                }
                State.Allowances[input.Symbol][User1Address][input.Spender] = input.Amount;
            });

            TokenContract.Setup(x => x.TransferFrom(It.IsAny<TransferFromInput>())).Callback<TransferFromInput>(input =>
            {
                State.Balances[input.Symbol][input.From] -= input.Amount;
                if (!State.Balances[input.Symbol].ContainsKey(input.To))
                {
                    State.Balances[input.Symbol][input.To] = 0;
                }
                State.Balances[input.Symbol][input.To] += input.Amount;
                State.Allowances[input.Symbol][input.From][User2Address] -= input.Amount;
            });
        }

        protected void Initialize()
        {
            TokenContract.Object.Initialize(new Empty());
        }

        private Address GenerateAddress()
        {
            var bytes = new byte[32];
            new Random().NextBytes(bytes);
            return new Address
            {
                Value = ByteString.CopyFrom(bytes)
            };
        }
    }

    public interface ITokenContract
    {
        TokenContractState State { get; }
        void Initialize(Empty input);
        void Create(TokenInfo input);
        void Issue(IssueInput input);
        void Transfer(TransferInput input);
        void Approve(ApproveInput input);
        void TransferFrom(TransferFromInput input);
    }
} 