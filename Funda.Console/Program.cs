// See https://aka.ms/new-console-template for more information

using Funda.ApiClient.Wcf;

Console.WriteLine("Hello, World!");

await new FundaWcfApiClient().ThrowsExceptions();
