/*  
    This assignment is created by :
    Kimberly Salemink   0995873    
    Regiena Zimmerman   0979691
    INF2A 
*/

using Sequential;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Program;

namespace Concurrent
{
    public class ConcurrentServer : SequentialServer
    {
        private Thread workerThread;
        private static Mutex mutex = new Mutex();

        public ConcurrentServer(Setting settings) : base(settings)
        {
            this.settings = settings;
            this.ipAddress = IPAddress.Parse(settings.serverIPAddress);
        }
        public override void prepareServer()
        {
            Console.WriteLine("[Server] is ready to start ...");
            try {
                localEndPoint = new IPEndPoint(this.ipAddress, settings.serverPortNumber);
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(settings.serverListeningQueue);
                while (true) {
                    Console.WriteLine("Waiting for incoming connections ... ");
                    Socket connection = listener.Accept();
                    this.numOfClients++;
                    this.handleClient(connection);
                }
            }
            catch (Exception e) { Console.Out.WriteLine("[Server] Preparation: {0}", e.Message); }

        }

        public override string processMessage(String msg)
        {
            string replyMsg = Message.confirmed;
            Thread.Sleep(settings.serverProcessingTime);

            if (mutex.WaitOne(5000)) {
                workerThread = new Thread(() => {
                    try {
                        switch (msg) {
                            case Message.terminate:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("[Server] received from the client -> {0} ", msg);
                                Console.ResetColor();
                                Console.WriteLine("[Server] END : number of clients communicated -> {0} ", this.numOfClients);
                                Thread.Sleep(5000);
                                mutex.ReleaseMutex();
                                break;
                            default:
                                replyMsg = Message.confirmed;
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.WriteLine("[Server] received from the client -> {0} ", msg);
                                Console.ResetColor();
                                Thread.Sleep(5000);
                                mutex.ReleaseMutex();
                                break;
                        }
                    }
                    catch (Exception e) { Console.Out.WriteLine("[Server] Process Message {0}", e.Message); }
                });
                workerThread.Start();
                workerThread.Join();
            }
            return replyMsg;
        }
    }
}