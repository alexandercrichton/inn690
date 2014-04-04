using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Veis.Unity.Simulation;

namespace Veis.Unity
{
    class Program
    {
        private static UnitySimulation simulation;

        static void Main(string[] args)
        {
            simulation = new UnitySimulation();

            while (true)
            {
                ReadInput();
            }
        }

        private static void ReadInput()
        {
            string input = Console.ReadLine();
            if (input.Length > 1)
            {
                simulation.Send(input);
            }            
        }    
    }
}
