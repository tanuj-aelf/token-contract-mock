# Mock System Contract

This repository contains a mock implementation of the AElf Token Contract system for testing purposes. It provides a simplified version of the token contract that can be used for local testing without requiring the full AElf infrastructure.

## Project Structure

- `token_contract.proto`: The protobuf definition file for the token contract
- `test/`: Contains the test implementation and mock contract
  - `TokenContractTestBase.cs`: Base test class with mock implementations
  - `TokenContractTests.cs`: Unit tests for the token contract
  - `TokenContractState.cs`: State management for the token contract
  - `AElf.Contracts.MultiToken.Tests.csproj`: Project file with dependencies

## Features

- Mock implementation of basic token operations:
  - Create token
  - Issue token
  - Transfer
  - Approve
  - TransferFrom

## Getting Started

1. Ensure you have .NET 6.0 SDK installed
2. Clone the repository
3. Run the tests:
   ```bash
   dotnet test test/AElf.Contracts.MultiToken.Tests.csproj
   ```

## Dependencies

- AElf.Types (1.3.0)
- AElf.Sdk.CSharp (1.3.0)
- AElf.Cryptography (1.3.0)
- Google.Protobuf (3.19.4)
- Moq (4.18.4)
- Other testing dependencies (xUnit, etc.) 