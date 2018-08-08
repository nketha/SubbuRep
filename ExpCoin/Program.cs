using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            decimal subbuCoin1 = 0;
            decimal subbuCoin2 = 0;
            decimal sendAmount = 0;
            int blockCount = 0;
            int option = 0;

            RandomUtils.Random = new UnsecureRandom();

            List<Transaction> transactionList = new List<Transaction>();
            DateTimeOffset current = DateTimeOffset.UtcNow;
            ConcurrentChain chain = new ConcurrentChain(Network.Main);

            Key subbuPrivateKey = new Key(); Key swethaPrivateKey = new Key();
            Key adityaPrivateKey = new Key(); Key kavyaPrivateKey = new Key();

            BitcoinSecret subbu = subbuPrivateKey.GetBitcoinSecret(Network.Main);
            BitcoinSecret swetha = swethaPrivateKey.GetBitcoinSecret(Network.Main);
            BitcoinSecret aditya = adityaPrivateKey.GetBitcoinSecret(Network.Main);
            BitcoinSecret kavya = kavyaPrivateKey.GetBitcoinSecret(Network.Main);

            Console.WriteLine("\n Enter first coin value:");
            subbuCoin1 = Convert.ToDecimal(Console.ReadLine());

            Console.WriteLine("\n Enter second coin value:");
            subbuCoin2 = Convert.ToDecimal(Console.ReadLine());

            decimal totalAmount = subbuCoin1 + subbuCoin2;
            Console.WriteLine("\n Total Coins Value: " + totalAmount);

            while (blockCount != 5) {

                while (option != 1) {

                    Console.WriteLine("\n Enter Send amount:");
                    sendAmount = Convert.ToDecimal(Console.ReadLine());
                    Transaction subbuFunding = new Transaction() {
                        Outputs =

    {

        new TxOut(subbuCoin1.ToString(), subbu.GetAddress()),
        new TxOut(subbuCoin2.ToString(), subbu.PubKey)
    }
                    };
                    Coin[] subbuCoins = subbuFunding
                                            .Outputs
                                            .Select((o, i) => new Coin(new OutPoint(subbuFunding.GetHash(), i), o))
                                            .ToArray();
                    var txBuilder = new TransactionBuilder();

                    var tx = txBuilder
                        .AddCoins(subbuCoins)
                        .AddKeys(subbu.PrivateKey)
                        .Send(swetha.GetAddress(), sendAmount.ToString())
                        .SendFees("0.001")
                        .SetChange(subbu.GetAddress())
                        .BuildTransaction(true);
                    Console.WriteLine(tx);
                    Console.ReadLine();
                    transactionList.Add(tx);
                    Console.WriteLine("Press 1 for Mine block or 2 go to next transaction:");
                    option = int.Parse(Console.ReadLine());
                }
                chain.SetTip(CreateBlock(current, blockCount, transactionList, chain));
                blockCount++;
                option = 0;
            }
        }
        private static ChainedBlock CreateBlock(DateTimeOffset current, int offset, List<Transaction> transactionList, ChainBase chain = null)
        {
            Block block = Consensus.Main.ConsensusFactory.CreateBlock();
            if (chain != null) {
                block.Header.HashPrevBlock = chain.Tip.HashBlock;
                block.Transactions.AddRange(transactionList);
                Console.WriteLine("Previous Block Hash" + block.Header.HashPrevBlock);
                Console.WriteLine("Created Block Hash:" + block.GetHash());
                Console.ReadLine();
                return new ChainedBlock(block.Header, null, chain.Tip);
            }
            else
                return new ChainedBlock(block.Header, 0);
        }
    }
}