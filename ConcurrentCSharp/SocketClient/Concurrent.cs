using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Sequential;

namespace Concurrent
{
    public class ConcurrentClient : SimpleClient
    {
        public Thread workerThread;

        public ConcurrentClient(int id, Setting settings) : base(id, settings)
        {
            // todo [Assignment]: implement required code
            this.settings = settings;
            client_id = id;
            cmd = this.settings.votingList;
            cmd_message = "ClientId=" + client_id.ToString() + settings.command_msg_sep + cmd;

            this.ipAddress = IPAddress.Parse(settings.serverIPAddress);
            waitingTime = new Random().Next(settings.clientMinStartingTime, settings.clientMaxStartingTime);
        }
        public void run()
        {
            

        }
    }
    public class ConcurrentClientsSimulator : SequentialClientsSimulator
    {
        private ConcurrentClient[] clients;
        private Thread workerThread;

        public ConcurrentClientsSimulator() : base()
        {
            Console.Out.WriteLine("\n[ClientSimulator] Concurrent simulator is going to start with {0}", settings.experimentNumberOfClients);
            clients = new ConcurrentClient[settings.experimentNumberOfClients];
            workerThread = new Thread(ConcurrentSimulation);
            workerThread.Start();
            workerThread.Join();
        }

        public void ConcurrentSimulation()
        {
            try
            {
                for (int i = 0; i < settings.experimentNumberOfClients; i++) {
                    clients[i].prepareClient();
                    clients[i].communicate();
                }

                Console.Out.WriteLine("\n[ClientSimulator] All clients finished with their communications ... ");

                Thread.Sleep(settings.delayForTermination);

                SimpleClient endClient = new SimpleClient(-1, settings); 
                endClient.prepareClient();
                endClient.communicate();
            }
            catch (Exception e)
            { Console.Out.WriteLine("[Concurrent Simulator] {0}", e.Message); }
        }
    }
}
