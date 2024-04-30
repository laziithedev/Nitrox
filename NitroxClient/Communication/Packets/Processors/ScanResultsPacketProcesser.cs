using NitroxClient.Communication.Packets.Processors.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroxClient.Communication.Packets.Processors
{
        public class ScanResultPacketProcessor : ClientPacketProcessor<ScanResultPacket>
        {
            public override void Process(ScanResultPacket packet)
            {
                // Update the MapRoom display with the new scan result
            }
        }
    }

