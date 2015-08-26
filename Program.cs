using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Internal;
using ColoredConsole;

namespace Akka.No
{
    class Program
    {
        private static void Main(string[] args)
        {
            //PricingHouse
            //Brokerage
            //Buyers
            //Sellers

            using (ActorSystem brokerage = ActorSystem.Create("Brokerage"))
            {


                ColorConsole.WriteLineGreen("Initialising Actor System");


                Props brokerActorProps = Props.Create<Broker>();
                Props playerActorProps = Props.Create<Buyer>();

                var iactorrefBuyer = brokerage.ActorOf(props: playerActorProps, name: "Buyer");
                var iactorrefBroker = brokerage.ActorOf(props: brokerActorProps, name: "Broker");


                var resultCollection = new ConcurrentBag<int>();
                var iterations = 10000;

                var result =    Parallel.For(0, iterations, (i) =>
                {
                    var buyMessage = new BuyMessage() {sender = iactorrefBroker,Symbol = "MIC", Units = i};

                    iactorrefBroker.Tell(buyMessage, iactorrefBuyer);
                    resultCollection.Add(i);

                });



                while (resultCollection.Count <  iterations)
                {

                    Console.Write("z");
                }
                Console.ReadLine();

                brokerage.Shutdown();

            }

        }



    }

    public class BuyMessage
    {
        public IActorRef sender { get; set; }
        public string Symbol { get; set; }
        public int Units { get; set; }
        
    }


    public class Broker : UntypedActor
    {

        protected override void OnReceive(object message)
        {
            Console.WriteLine(message);

            if (message is BuyMessage)
            {
                var buy = message as BuyMessage;
                buy.sender.Tell("OK got " + message);
            }

        }
    }


    public class Buyer : UntypedActor
    {



        protected override void OnReceive(object message)
        {
           Console.WriteLine(message);
        }
    }

}
