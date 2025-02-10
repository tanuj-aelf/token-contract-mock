using System.Threading.Tasks;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;

namespace AElf.Contracts.MultiToken
{
    public class TokenContractTests : TokenContractTestBase
    {
        [Fact]
        public void Initialize_Test()
        {
            Initialize();
            State.ShouldNotBeNull();
        }

        [Fact]
        public void Create_Token_Test()
        {
            Initialize();
            var tokenInfo = new TokenInfo
            {
                Symbol = "TEST",
                TokenName = "TEST Token",
                TotalSupply = 1_000_000_000,
                Decimals = 8,
                Issuer = DefaultAddress,
                IsBurnable = true
            };
            TokenContract.Object.Create(tokenInfo);
            State.TokenInfos.ContainsKey("TEST").ShouldBeTrue();
            State.TokenInfos["TEST"].Symbol.ShouldBe("TEST");
            State.TokenInfos["TEST"].TokenName.ShouldBe("TEST Token");
            State.TokenInfos["TEST"].TotalSupply.ShouldBe(1_000_000_000);
        }

        [Fact]
        public void Issue_Token_Test()
        {
            Create_Token_Test();
            var issueInput = new IssueInput
            {
                Symbol = "TEST",
                Amount = 100,
                To = User1Address
            };
            TokenContract.Object.Issue(issueInput);
            State.Balances["TEST"][User1Address].ShouldBe(100);
        }

        [Fact]
        public void Transfer_Token_Test()
        {
            Issue_Token_Test();
            var transferInput = new TransferInput
            {
                Symbol = "TEST",
                Amount = 50,
                To = User2Address,
                Memo = "transfer test"
            };
            TokenContract.Object.Transfer(transferInput);
            State.Balances["TEST"][User1Address].ShouldBe(50);
            State.Balances["TEST"][User2Address].ShouldBe(50);
        }

        [Fact]
        public void Approve_And_TransferFrom_Test()
        {
            Issue_Token_Test();
            var approveInput = new ApproveInput
            {
                Symbol = "TEST",
                Amount = 50,
                Spender = User2Address
            };
            TokenContract.Object.Approve(approveInput);
            State.Allowances["TEST"][User1Address][User2Address].ShouldBe(50);

            var transferFromInput = new TransferFromInput
            {
                Symbol = "TEST",
                Amount = 50,
                From = User1Address,
                To = User2Address,
                Memo = "transferFrom test"
            };
            TokenContract.Object.TransferFrom(transferFromInput);
            State.Balances["TEST"][User1Address].ShouldBe(50);
            State.Balances["TEST"][User2Address].ShouldBe(50);
            State.Allowances["TEST"][User1Address][User2Address].ShouldBe(0);
        }

        protected new Address DefaultAddress => base.DefaultAddress;
    }
} 