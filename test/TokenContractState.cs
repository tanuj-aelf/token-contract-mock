using System.Collections.Generic;
using AElf.Types;

namespace AElf.Contracts.MultiToken
{
    public class TokenContractState
    {
        public long ChainId { get; set; }
        public Address Owner { get; set; }
        public Dictionary<string, TokenInfo> TokenInfos { get; set; } = new Dictionary<string, TokenInfo>();
        public Dictionary<string, Dictionary<Address, long>> Balances { get; set; } = new Dictionary<string, Dictionary<Address, long>>();
        public Dictionary<string, Dictionary<Address, Dictionary<Address, long>>> Allowances { get; set; } = new Dictionary<string, Dictionary<Address, Dictionary<Address, long>>>();
    }
} 