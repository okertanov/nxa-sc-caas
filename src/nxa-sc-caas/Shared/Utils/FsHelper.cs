using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NXA.SC.Caas.Shared.Utils
{
    public static class FsHelper
    {
        private static ILogger? logger = ApplicationLogging.CreateLogger(nameof(FsHelper));

        public static async Task DeleteFolder(string folderPath)
        {
            try
            {
                await Task.Factory.StartNew(path =>
                {
                    if (Directory.Exists((string)path!))
                    {
                        Directory.Delete((string)path!, true);
                    }
                }, folderPath);

                logger?.LogInformation($"Directory {folderPath} deleted");
            }
            catch (Exception e)
            {
                logger?.LogWarning(e.StackTrace);
            }
        }

        public static async Task PrepareDirForHhCompile(string taskPath, string contractName, string sourceStrNormalized)
        {
            await CreateContractDirAndFile(taskPath, contractName, sourceStrNormalized);
            await CreateHhConfigFile(taskPath);
        }

        private static async Task CreateContractDirAndFile(string taskPath, string contractName, string sourceStrNormalized)
        {
            var contractPath = $@"{taskPath}/contracts";
            await Task.Factory.StartNew(path => Directory.CreateDirectory((string)path!), contractPath);
            logger?.LogInformation($"Directory {taskPath} created");
            await Task.Factory.StartNew(path => File.WriteAllText(Path.Combine((string)path!, $"{contractName}.sol"), sourceStrNormalized), contractPath);
            logger?.LogInformation($"File {contractName}.sol created");
        }

        private static async Task CreateHhConfigFile(string taskPath)
        {
            var hhConfigSrc = @"require('@nomiclabs/hardhat-etherscan');
                                require('@nomiclabs/hardhat-ethers');
                                require('@nomiclabs/hardhat-web3');
                                require('@openzeppelin/hardhat-upgrades');

                                module.exports = {
                                  paths: {
                                    artifacts: './artifacts',
                                    cache: './cache',
                                    sources: './contracts'
                                  },
                                  solidity: {
                                    compilers: [
                                      {
                                        version: '0.8.4',
                                        settings: {
                                          optimizer: {
                                            enabled: true,
                                            runs: 200,
                                          },
                                        },
                                      },
                                    ],
                                  }
                                }
                                ";

            await Task.Factory.StartNew(path => File.WriteAllText(Path.Combine((string)path!, "hardhat.config.js"), hhConfigSrc), taskPath);
            logger?.LogInformation($"File hardhat.config.js created");
        }
    }
}
