namespace bitcoin_from_scratch
{
    public class Program
    { 
        public static void Main(string[] args)
        {
            var wallet = new Wallet();
            Console.WriteLine(wallet.GenerateBitcoinAddress());
        }
    }
}
